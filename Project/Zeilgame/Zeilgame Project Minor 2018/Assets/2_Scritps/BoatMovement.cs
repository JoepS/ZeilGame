using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoatMovement : IBoat {

    public const string SimpleOrAdvancedSettingKey = "SimpleOrAdvanced";
    bool _simpleOrAdvancedSetting = false; //False = simple; True = advanced


    // Use this for initialization
    void Start () {
        string simpleoradvance = PlayerPrefs.GetString(SimpleOrAdvancedSettingKey);
        if (simpleoradvance.Equals("simple"))
        {
            _simpleOrAdvancedSetting = true;
        }
        else
        {
            _simpleOrAdvancedSetting = false;
        }
        _tackingExtra = 0f;
        _boat = MainGameController.instance.player.GetActiveBoat();
        _hullSpeed = _boat.GetSpeedModified();
        Debug.Log("NogoZoneLimit: " + _boat.GetNGZLimit() + " / " + _boat.GetNGZLimitModified());
        _ngzLimit = _boat.GetNGZLimitModified();
        _tempLineRenderer = this.GetComponent<LineRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (_canMove)
        {
            Movement();
            SailRotation();
        }
	}

    void Movement()
    {
        GameObject temp = _target;
        bool leftOfTargetUpwind = /*_upwindBoat &&*/ _upwindTarget && (this.transform.localPosition.x < _otherTarget.transform.localPosition.x);

        if (leftOfTargetUpwind)
        {
            _target = _otherTarget;
        }
        if (!_simpleOrAdvancedSetting)
        {
            if (Input.GetMouseButton(0))
            {
                Rotation();
            }
            MoveBoat();
        }
        else
        {
            BoatMovement();
        }

        _target = temp;

        CalculateOptimalLineUpwind();
    }

    void MoveBoat()
    {

        float myangle = GetAngleTwWind();
        _upwindTarget = this.transform.position.y < _target.transform.position.y ? true : false;
        _upwindBoat = myangle > 90 && myangle < 270 ? false : true;

        float angleTwWind = GetAngleTwWind();
        if (angleTwWind > 180)
            angleTwWind = 360 - angleTwWind;
        float windSpeed = RaceSceneController.instance.windSpeed;
        _speed = calculateForwardSpeed(angleTwWind, windSpeed);
        _speed = MainGameController.Map(_speed, calculateForwardSpeed(1, windSpeed), calculateForwardSpeed(130, windSpeed), _minVelocity, _maxVelocity);
        _velocity = this.transform.up * _speed;
        SailRotation();
    }

    public void Rotation()
    {
        Vector3 mousePos = Input.mousePosition;
        _rotationSpeed = 10;
        float rotationDirection = 5;
        if (mousePos.x < Screen.width / 2)
        {
            rotationDirection *= -1;
        }
        else
        {
            rotationDirection = Mathf.Abs(rotationDirection);
        }

        Quaternion rot = Quaternion.Lerp(this.transform.rotation,
                                         Quaternion.Euler(this.transform.eulerAngles - new Vector3(0, 0, rotationDirection)),
                                         Time.deltaTime * _rotationSpeed);
        this.transform.rotation = rot;
    }

    public void OnMouseButtondDown()
    {
        if (_simpleOrAdvancedSetting)
        {
            _starboard = !_starboard;
            GoTack();
        }
    }

    public float GetMaxWindSpeedSails()
    {
        return _boat.GetMaxWindSpeedSails();
    }

    public IEnumerator ReceiveDamage()
    {
        while (true)
        {
            if (canMove)
            {
                _boat.Damage += Random.Range(0f, 1f);
                _boat.Save();
                float waitTime = Random.Range(30, 120);
                yield return new WaitForSeconds(waitTime);
            }
            yield return new WaitForSeconds(1);
        }
    }
}
