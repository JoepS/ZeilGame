using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

    [SerializeField, Range(1, 100)]protected float _hullSpeed;

    protected float _vic = 10;
    [SerializeField, Range(1, 179)] protected float _ngzLimit = 40;
    protected float _degreeInterval = 5;

    protected float _bestWindAngle = 130;

    protected float _minVelocity = float.MinValue;
    protected float _maxVelocity = float.MaxValue;

    public float _speed;
    [SerializeField, Range(1, 50)] protected float _rotationSpeed;

    public GameObject target { get { return _target; } set { _target = value; } }
    [SerializeField] protected GameObject _target;

    public Vector2 velocity { get { return _velocity; } }
    protected Vector2 _velocity;

    protected Boat _boat;

    public bool _tacking = false;
    protected float _tackingAccuracy = 5f;
    protected float _tackingExtra = 10f;
    [SerializeField] protected float _tackingSpeed = 10;

    protected bool _upwindBoat;
    protected bool _upwindTarget;

    [SerializeField] float _windSpeed;
    [SerializeField] float _windangle;

    [SerializeField] GameObject _player;
    [SerializeField] GameObject _buoy;

    GameObject parent;
    GameObject parent2;

    [SerializeField, Range(1, 180)] int targetAngle;
    [SerializeField, Range(1, 180)] int windAngle;
    [SerializeField] float _currentSpeed;

    // Use this for initialization
    void Start ()
    {
        test();
        //StartCoroutine(CoroutineTest());
    }
	
	// Update is called once per frame
	void Update () {
        //BoatMovement();
        //test();
	}

    IEnumerator CoroutineTest()
    {
        while (true)
        {
            test();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void test()
    {
        Destroy(parent);
        Destroy(parent2);
        parent = new GameObject("VMG Parent");
        parent2 = new GameObject("Speed Parent");
        float prevspeed = 0;
        for (int i = 1; i < 180; i++)
        {
            float windangle = 45;//(i - windAngle);
            if (windangle < 0)
                windangle *= -1;
            if (windangle == 0)
                windangle = 1;

            GameObject speedgo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            speedgo.transform.localScale = new Vector3(10, 10, 10);
            speedgo.name = "WA:" + windangle;
            speedgo.transform.SetParent(parent2.transform);
            float forwardForce = calculateForwardSpeed(windangle, 10, prevspeed, windangle < 90);
            float reactiveForce = 0.5f * 1f * Mathf.Pow(forwardForce, 2) * 0.09f;
            float acceleration = forwardForce - reactiveForce;
            Vector3 speedpos = new Vector3(i * 10, acceleration);
            speedgo.transform.position = speedpos;
            prevspeed = acceleration;


            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.localScale = new Vector3(10, 10, 10);
            go.name = "WA:" + windangle;
            go.transform.SetParent(parent.transform);
            Vector3 pos = new Vector3(i * 10, reactiveForce);
            go.transform.position = pos;
        }
    }

    public float VelocityMadeGood(float windSpeed, float angleWind, float angleDirection, bool upwind, float currentSpeed)
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
        float speed = calculateForwardSpeed(angleWind, windSpeed, currentSpeed, upwind);
        float newVMG = speed * vmg;
        if (speed < 0 && newVMG > 0)
            newVMG *= -1;
        return newVMG;
    }

    public float calculateForwardSpeed(float angleToWind, float windSpeed, float currentSpeed, bool upwind)
    {
        float tw = windSpeed;
        float b = angleToWind;
        if(!upwind)
            b = 180 - b;

        b *= Mathf.Deg2Rad;
        
        float twpow = Mathf.Pow(tw, 2);
        float cspow = Mathf.Pow(currentSpeed, 2);
        float twoWV = 2 * tw * currentSpeed;
        float cosb = Mathf.Cos(b);


        float aparantspeed = Mathf.Sqrt(twpow + cspow + twoWV * cosb);
;
        float a = Mathf.Acos((tw * Mathf.Cos(b) + currentSpeed) / aparantspeed);

        float x = (Mathf.Sin(b) * Mathf.Cos(a)) / Mathf.Sin(a);
        x = Mathf.Cos(a) * aparantspeed;

        float bs;
        if (upwind)
            bs = x - Mathf.Cos(b);
        else
            bs = x + Mathf.Cos(b);
       
        return bs;

    }

    public float GetAngleTwWind()
    {
        float angle = this.transform.localEulerAngles.z + (360 - _windangle);

        if (angle > 360)
            angle -= 360;
        return angle;
    }
}
