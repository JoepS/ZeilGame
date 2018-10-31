using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentRaceAi : IBoat {

    public List<GameObject> buoys { set { _buoys = value; } }
    List<GameObject> _buoys;
    public List<int> trackOrder { set { _trackOrder = value; } }
    List<int> _trackOrder;
    public int targetCounter { get { return _targetCounter; } }
    int _targetCounter = 0;

    public float _targetAngle;


    bool _keepGoingSameDirection = false;
    GameObject _finishTarget;

    public Person person { get { return _person; } }
    Person _person;

    public void SetOpponentValues(Person person, float TackingAccuracy, float TackingExtra, float TackingSpeed, float NGZLimit, float BestUpwindAngle)
    {
        _person = person;
        _tackingAccuracy = TackingAccuracy;
        _tackingExtra = TackingExtra;
        _tackingSpeed = TackingSpeed;
        _ngzLimit = NGZLimit;
        _bestUpwindAngle = BestUpwindAngle;
    }

	// Use this for initialization
	void Start () {
        _boat = MainGameController.instance.databaseController.connection.Table<Boat>().Where(x => x.id == _person.BoatId).First();
        _hullSpeed = _boat.GetSpeed();
        _tempLineRenderer = this.GetComponent<LineRenderer>();
    }

	// Update is called once per frame
	void Update () {
        if (_canMove)
            Movement();
	}
    void Movement()
    {
        GameObject temp = _target;
        bool leftOfTargetUpwind = /*_upwindBoat &&*/ _upwindTarget && (this.transform.localPosition.x < _otherTarget.transform.localPosition.x);

        if (leftOfTargetUpwind && !_keepGoingSameDirection)
        {
           _target = _otherTarget;
        }
        BoatMovement();

        if (CanTack() && !_tacking && !_keepGoingSameDirection)
        {
            GoTack();
        }
        _target = temp;
        Vector2 pos = this.transform.localPosition;
        pos += _velocity;
        this.transform.localPosition = pos;

        CalculateOptimalLineUpwind();

        CheckTargetPassed();
    }

    public void CheckTargetPassed()
    {
        if (_keepGoingSameDirection)
            return;
        bool passingLine = false;
        if (_targetCounter < 2 || _targetCounter >= _trackOrder.Count - 2)
        {
            passingLine = CheckIfPassedThroughLineBetween(_buoys[_trackOrder[_targetCounter]],
                                                          _buoys[_trackOrder[_targetCounter + 1]]);
        }
        if((CheckIfBuoyPassed() && _targetCounter > 1 && _targetCounter < _trackOrder.Count - 2) || passingLine)
        {
            if (passingLine)
                _targetCounter++;
            _targetCounter++;
            if (_targetCounter > _trackOrder.Count - 2)
            {
                RaceSceneController.instance.OpponentFinished(this);
                _keepGoingSameDirection = true;
                _target = _finishTarget;
            }
            else
            {
                WaypointBuoy newBuoy = _buoys[_trackOrder[_targetCounter]].GetComponent<WaypointBuoy>();
                if (newBuoy.transform.localPosition.y > this.transform.localPosition.y)
                    this.target = newBuoy.upwindTarget;
                else
                    this.target = newBuoy.downwindTarget;


            }
        }
    }


    bool CanTack()
    {
        Vector3 relativePos = _target.transform.position - this.transform.position;
        _targetAngle = Vector2.Angle(this.transform.up, relativePos);

        Vector3 diff = this.transform.position - _target.transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        float AngleTargetTwBoat = rot_z - 90;

        float targetAngleRelativeToWind = AngleTargetTwBoat + 180;
        if (targetAngleRelativeToWind < 0)
            targetAngleRelativeToWind = 360 + targetAngleRelativeToWind;

        float angleTwTargetOffWind = Mathf.Abs(GetAngleTwWind() - targetAngleRelativeToWind);

        bool TargetAngleBiggerBestUpwindAngle = _targetAngle >= _bestUpwindAngle;
        if (!_upwindBoat)
            TargetAngleBiggerBestUpwindAngle = _targetAngle >= 5;
        bool TargetToTheRight = this.transform.position.x < _target.transform.position.x;//AngleDir(this.transform.forward, relativePos, this.transform.up) == 1;
        bool TargetLeftButNotGoinToward = (!TargetToTheRight && angleTwTargetOffWind > _bestUpwindAngle);
        bool JustLuffing = (GetAngleTwWind() > 180 && TargetToTheRight) || (GetAngleTwWind() < 180 && !TargetToTheRight);//(_upwindBoat != _upwindTarget);
        bool JustFalling = JustLuffing &&  (!_upwindBoat && !_upwindTarget);
        

        if (TargetAngleBiggerBestUpwindAngle && (TargetToTheRight || TargetLeftButNotGoinToward) && !JustLuffing && !JustFalling)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //Left -1
    //Center 0
    //Right 1
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

    public void SetFinishTarget(GameObject finishTarget)
    {
        _finishTarget = finishTarget;
    }
}
