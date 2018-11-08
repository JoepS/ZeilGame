using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IBoat : MonoBehaviour {

    protected float _hullSpeed;

    protected float _vic = 10;
    protected float _ngzLimit = 40;
    protected float _degreeInterval = 5;

    protected float _bestWindAngle = 130;

    protected float _minVelocity = float.MinValue;
    protected float _maxVelocity = float.MaxValue;

    protected float _speed;
    [SerializeField]protected float _rotationSpeed;

    public GameObject target { get { return _target; } set { _target = value; } }
    [SerializeField, Header("Target to sail towards")] protected GameObject _target;

    public Vector2 velocity { get { return _velocity; } }
    protected Vector2 _velocity;

    protected Boat _boat;

    protected bool _tacking = false;
    [Header("Tacking Properties")]
    [SerializeField] protected float _tackingAccuracy = 5f;
    [SerializeField] protected float _tackingExtra = 10f;
    [SerializeField] protected float _tackingSpeed = 10;
    
    [SerializeField, Header("Target for best upwind angle")]
    protected GameObject _otherTarget;

    [SerializeField, Header("Angle for best upwind course")]
    protected float _bestUpwindAngle;

    protected bool _upwindBoat;
    protected bool _upwindTarget;

    public bool canMove
    {
        get
        {
            return _canMove;
        }
        set
        {
            _canMove = value;
        }
    }
    [SerializeField] protected bool _canMove = false;


    [SerializeField] GameObject _sailImage;
    protected bool _starboard = false;
    
    protected LineRenderer _tempLineRenderer;

    public void BoatMovement()
    {
        float angleTwWind = GetAngleTwWind();
        if (angleTwWind > 180)
            angleTwWind = 360 - angleTwWind;

        float windSpeed = RaceSceneController.instance.windSpeed;

        _speed = calculateForwardSpeed(angleTwWind, windSpeed);

        //float forwardForce = calculateForwardSpeed(angleTwWind, 10);
        //float reactiveForce = 0.5f * 1f * Mathf.Pow(forwardForce, 2) * 0.09f;
        //float acceleration = forwardForce - reactiveForce;
        //_speed += acceleration;

        if (_speed == float.NegativeInfinity)
            _speed = calculateForwardSpeed(1, windSpeed);
        if (_speed == float.PositiveInfinity)
            _speed = calculateForwardSpeed(130, windSpeed);

        _speed = MainGameController.Map(_speed, calculateForwardSpeed(1, windSpeed), calculateForwardSpeed(130, windSpeed), _minVelocity, _maxVelocity);
        if (_speed == float.NaN || float.IsNaN(_speed))
            _speed = calculateForwardSpeed(1, windSpeed);

        float rotateSpeed = 5;
        float vmgRotateSpeed = 1;

        Vector3 relativePos = _target.transform.position - this.transform.position;
        float angle = Vector2.Angle(this.transform.up, relativePos);

        _rotationSpeed = MainGameController.Map(angle, 0, 180, 1, 20);

        float myangle = GetAngleTwWind();
        _upwindTarget = this.transform.position.y < _target.transform.position.y ? true : false;
        _upwindBoat = myangle > 90 && myangle < 270 ? false : true;

        if (!_upwindTarget)
            vmgRotateSpeed *= -1;

        if (myangle > 180)
        {
            rotateSpeed *= -1;
        }

        float vmg = (VelocityMadeGood(windSpeed, angleTwWind, angle, (angleTwWind < 90)));
        float vmgUp = (VelocityMadeGood(windSpeed, angleTwWind, angle - vmgRotateSpeed, (angleTwWind + vmgRotateSpeed < 90)));
        float vmgDown = (VelocityMadeGood(windSpeed, angleTwWind, angle + vmgRotateSpeed, (angleTwWind - vmgRotateSpeed < 90)));
        float twTargetWindangle = angleTwWind;// - angle;

        Vector3 other = _target.transform.position - this.transform.position;

        float angledir = Vector2.Dot(this.transform.right, other);

        float tempTwTargetThingy = 1;
        if (myangle > 180)
        {
            tempTwTargetThingy *= -1;
        }

        //left  < 0
        //right > 0
        if ((angledir < 0)){
            twTargetWindangle += angle * tempTwTargetThingy;
            //Debug.Log("Left");
        }
        else
        {
            twTargetWindangle -= angle * tempTwTargetThingy;
            //Debug.Log("Right");
        }
        if (twTargetWindangle < 0)
            twTargetWindangle *= -1;
        if (twTargetWindangle > 180)
            twTargetWindangle = 360 - twTargetWindangle;
        
        float vmgTowardsTarget = VelocityMadeGood(windSpeed, twTargetWindangle, 0, angleTwWind < 90);

        if (vmgUp == float.PositiveInfinity || vmgUp == float.NegativeInfinity)
            vmgUp = float.MinValue;
        if (vmgDown == float.PositiveInfinity || vmgUp == float.NegativeInfinity)
            vmgDown = float.MinValue;

        

        bool angleTwWindSmallerThenAngle = (angleTwWind < angle);

        if (myangle < 180)
            angledir *= -1;
        if ((vmgTowardsTarget > vmgUp || vmgTowardsTarget > vmgDown) && !angleTwWindSmallerThenAngle)
        {
            if (angledir < 0)
            {
                //left
                vmgUp = float.MaxValue;
            }
            else if (angledir > 0)
            {
                //right
                vmgDown = float.MaxValue;
            }
            else
            {
                //Infront
                vmg = float.MaxValue;
            }
        }

        if (!_tacking)
        {
            if (vmg < vmgUp && vmgUp > vmgDown)// && updiff > minimumDiff)
            {
                Quaternion rot = Quaternion.Lerp(this.transform.rotation,
                                                 Quaternion.Euler(this.transform.eulerAngles - new Vector3(0, 0, rotateSpeed)),
                                                 Time.deltaTime * _rotationSpeed);

                float currRot = this.transform.rotation.eulerAngles.z;
                if (this.gameObject.name.Equals("Player"))
                {
                    if (currRot > 180)
                    {
                        if (rot.eulerAngles.z > 180)
                            this.transform.rotation = rot;
                    }
                    else if (currRot < 180)
                    {
                        if (rot.eulerAngles.z < 180)
                            this.transform.rotation = rot;
                    }
                }
                else
                {
                    this.transform.rotation = rot;
                }
            }
            else if (vmg < vmgDown && vmgDown > vmgUp)// && downdiff > minimumDiff)
            {
                Quaternion rot = Quaternion.Lerp(this.transform.rotation,
                                                          Quaternion.Euler(this.transform.eulerAngles + new Vector3(0, 0, rotateSpeed)),
                                                          Time.deltaTime * _rotationSpeed);
                float currRot = this.transform.rotation.eulerAngles.z;
                if (this.gameObject.name.Equals("Player"))
                {
                    if (currRot > 180)
                    {
                        if (rot.eulerAngles.z > 180)
                            this.transform.rotation = rot;
                    }
                    else if (currRot < 180)
                    {
                        if (rot.eulerAngles.z < 180)
                            this.transform.rotation = rot;
                    }
                }
                else
                {
                    this.transform.rotation = rot;
                }
            }
            else
            {
            }
        }
        _velocity = this.transform.up * _speed;
        SailRotation();
    }

    public float VelocityMadeGood(float windSpeed, float angleWind, float angleDirection, bool upwind)
    {
        float a = (windSpeed / (Mathf.Cos(_ngzLimit * Mathf.Deg2Rad)));
        float b = 1 + (_vic / 100);
        float c;
        if (upwind)
            c = Mathf.Abs(((angleWind * Mathf.Deg2Rad) - (_ngzLimit * Mathf.Deg2Rad))) / _degreeInterval;
        else
            c = Mathf.Abs((180 - _ngzLimit - angleWind) * Mathf.Deg2Rad) / _degreeInterval;
        float d = Mathf.Cos(angleDirection * Mathf.Deg2Rad);

        float vmg = (a * Mathf.Pow(b, c) * d);
        float speed = calculateForwardSpeed(angleWind, windSpeed);
        float newVMG = speed * vmg;
        if (speed < 0 && newVMG > 0)
            newVMG *= -1;
        return newVMG;

    }

    public float vmgRateOfChange(float windSpeed, float angleWind, float angleDirection, bool upwind)
    {
        float a = (-windSpeed / (Mathf.Cos(_ngzLimit * Mathf.Deg2Rad)));
        float b = 1 + (_vic / 100);
        float c;
        if (upwind)
            c = Mathf.Abs(angleWind - _ngzLimit) / _degreeInterval;
        else
            c = Mathf.Abs(180 - _ngzLimit - angleWind) / _degreeInterval;
        float d = Mathf.Sin(angleDirection * Mathf.Deg2Rad);

        float vmg = (a * Mathf.Pow(b, c) * d);
        float speed = calculateForwardSpeed(angleWind, windSpeed);
        float newVMG = speed * vmg;
        if (speed < 0 && newVMG > 0)
            newVMG *= -1;


        return newVMG;
    }

    public float calculateForwardSpeed(float angleToWind, float windSpeed)
    {
        float speed = 0;
        if (angleToWind > 180)
            angleToWind = 360 - angleToWind;
        _minVelocity = 0;// -((_hullSpeed / (180-_ngzLimit)) * _ngzLimit);
        _maxVelocity = _hullSpeed + ((_hullSpeed / (180 - _ngzLimit)) * _ngzLimit) * (windSpeed * 0.25f);
        if (angleToWind < _bestWindAngle)
            speed = Mathf.Lerp(_minVelocity, _maxVelocity, angleToWind / _bestWindAngle);
        else
            speed = Mathf.Lerp(_maxVelocity, _maxVelocity * 0.6f, (angleToWind - _bestWindAngle) / 50);
        speed = Mathf.Log(speed) * windSpeed;
        return speed;

    }
    
    public float GetAngleTwWind()
    {
        float angle = this.transform.localEulerAngles.z + (360 - RaceSceneController.instance.windAngle);

        if (angle > 360)
            angle -= 360;
        return angle;
    }

    protected IEnumerator Tack(float newangle)
    {
        Vector3 angles = this.transform.localEulerAngles;
        angles.z = newangle;


        float tempaccuracy = _tackingAccuracy;
        if (!_upwindBoat)
        {
            _tackingAccuracy = 0.5f;
        }
        
        while (!(Mathf.Abs(this.transform.localEulerAngles.z - newangle) <= _tackingAccuracy || Mathf.Abs(this.transform.localEulerAngles.z - (360 + newangle)) <= _tackingAccuracy) &&
                _tacking)
        {
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.Euler(angles), Time.deltaTime * _tackingSpeed);
            yield return new WaitForSeconds(0.01f);
        }
        _tackingAccuracy = tempaccuracy;
        _tacking = false;
        AfterTack();
    }
    

    public bool CheckIfBuoyPassed()
    {
        //float dist = Vector2.Distance(this.transform.position, _target.transform.position);
        //if (dist > 500)
            //return false;
        if (_upwindTarget)
        {
            if ((this.transform.position.x > _target.transform.parent.position.x &&
                this.transform.position.y > _target.transform.parent.position.y)) //&&
                //dist < 512)
                return true;
        }
        else
        {
            if ((this.transform.position.x < _target.transform.parent.position.x &&
                this.transform.position.y < _target.transform.parent.position.y))
                return true;
        }
        return false;
    }

    public bool CheckIfPassedThroughLineBetween(GameObject buoyA, GameObject buoyB)
    {
        if (this.transform.position.x > buoyA.transform.position.x && this.transform.position.x < buoyB.transform.position.x)
        {
            float distA = Vector2.Distance(this.transform.position, buoyA.transform.position);
            float distB = Vector2.Distance(this.transform.position, buoyB.transform.position);

            float distaAB = Vector2.Distance(buoyA.transform.position, buoyB.transform.position);

            if ((distA + distB - distaAB) < 10 && (distA + distB - distaAB) > 0)
            {
                return true;
            }
        }

        return false;
    }

    protected void GoTack()
    {
        if (!_tacking)
        {
            float newangle = 0;

            float angleTwWind = GetAngleTwWind();

            Vector3 relativePos = _target.transform.position - this.transform.position;
            float angleTwTarget = Vector2.Angle(-Vector3.up, relativePos);
            angleTwTarget += _tackingExtra;

            float windAngle = RaceSceneController.instance.windAngle;

            if (angleTwWind > 270)
            {
                if(_upwindTarget)
                    newangle = windAngle + (_ngzLimit + _tackingExtra);
                else
                    newangle = Mathf.Abs(180 - windAngle) - (angleTwTarget + _tackingExtra);
            }
            else if (angleTwWind < 90)
            {
                if(_upwindTarget)
                    newangle = windAngle - (_ngzLimit + _tackingExtra);
                else
                    newangle = 180 + windAngle + (angleTwTarget + _tackingExtra);
            }
            else if (angleTwWind < 270 && angleTwWind > 180)
            {
                newangle = Mathf.Abs(180 - windAngle) - (angleTwTarget+ _tackingExtra);
            }
            else if (angleTwWind > 90 && angleTwWind < 180)
            {
                newangle = 180 + windAngle + (angleTwTarget + _tackingExtra);
            }
            if (newangle > 360)
                newangle -= 360;
            else if (newangle < -360)
                newangle += 360;
            _tacking = true;
            
            StartCoroutine(Tack(newangle));
        }
    }
    public void SailRotation()
    {
        float windangle = RaceSceneController.instance.windAngle;
        float windangleonboat = windangle - this.transform.localEulerAngles.z;
        if (windangleonboat < 0)
            windangleonboat += 360;
        windangleonboat = Mathf.RoundToInt(windangleonboat);

        if (windangleonboat >= 180)
        {
            //Bakboord
            float sailangle = MainGameController.Map(windangleonboat, 180, 360, 180, 260);
            Vector3 angles = new Vector3(0, 0, sailangle);
            _sailImage.transform.localEulerAngles = Vector3.Lerp(_sailImage.transform.localEulerAngles, angles, Time.deltaTime);
        }
        else
        {
            //Stuurboord
            float sailangle = MainGameController.Map(windangleonboat, 0, 180, 280, 360);
            Vector3 angles = new Vector3(0, 0, sailangle);
            _sailImage.transform.localEulerAngles = Vector3.Lerp(_sailImage.transform.localEulerAngles, angles, Time.deltaTime);
        }
    }

    public float getDistanceTillNextBuoy()
    {
        return Vector3.Distance(this.transform.position, _target.transform.position);
    }

    float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        //Left -1
        //Center 0
        //Right 1
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

    public virtual void AfterTack() { }

    public void CalculateOptimalLineUpwind()
    {

        float angleCA = _bestUpwindAngle + RaceSceneController.instance.windAngle;
        Vector3 buoyPos = _target.transform.parent.position;
        Vector2 mypos;

        float distance = Mathf.Abs(this.transform.position.y - buoyPos.y);//Vector2.Distance(this.transform.position, buoyPos); //C
        float tempangle = RaceSceneController.instance.windAngle;
        int overstaandNegatief = 1;
        if (tempangle > 90 && tempangle < 180)
        {
            tempangle = 180 - tempangle;
            overstaandNegatief = -1;
        }
        else if (tempangle < 270 && tempangle > 180)
        {
            tempangle = 360 - tempangle + 180;
            overstaandNegatief = -1;
        }
        float overstaand = Mathf.Sin(tempangle * Mathf.Deg2Rad) * distance; //A
        float aanliggend = Mathf.Sqrt(Mathf.Pow(distance, 2) + Mathf.Pow(overstaand, 2));//B

        mypos = buoyPos - new Vector3(-overstaand, aanliggend * overstaandNegatief);


        float c = Vector2.Distance(mypos, buoyPos);

        float a = c * Mathf.Cos(angleCA * Mathf.Deg2Rad);

        Vector2 pos = new Vector2();
        float d = Mathf.Sin(angleCA * Mathf.Deg2Rad) * a;

        pos.x = mypos.x + d;
        if (_upwindTarget)
        {
            pos.y = buoyPos.y - (a * Mathf.Cos(angleCA * Mathf.Deg2Rad));
        }
        else
        {
            pos.y = buoyPos.y + (a * Mathf.Cos(angleCA * Mathf.Deg2Rad));
        }
        _otherTarget.transform.position = pos;

        Vector3 euler = _otherTarget.transform.localEulerAngles;
        euler.z = Mathf.Acos(a / c) * Mathf.Rad2Deg;
        _otherTarget.transform.localEulerAngles = euler;

        _tempLineRenderer.positionCount = 3;
        _tempLineRenderer.SetPosition(0, mypos);
        _tempLineRenderer.SetPosition(1, pos);
        _tempLineRenderer.SetPosition(2, buoyPos);
        //Debug.Break();

    }
}
