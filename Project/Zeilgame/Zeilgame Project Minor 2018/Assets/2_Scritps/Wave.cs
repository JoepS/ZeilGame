using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour {

    bool _upDown = false; // false == down; true == up;

    Vector3 _direction;
    float _speed;

    public static bool CanMove = false;



    public void SetMovement(Vector3 dir, float speed, float startingSize)
    {
        this.transform.localRotation = Quaternion.Euler(dir);
        _speed = speed;
        Vector3 scale = this.transform.localScale;
        scale.y = startingSize;
        this.transform.localScale = scale;
    }

	
	// Update is called once per frame
	void Update () {
        if (CanMove)
        {
            Move();
        }
	}

    void Move()
    {
        if (_upDown)
        {
            Vector3 scale = this.transform.localScale;
            scale.y += Time.deltaTime;
            if (scale.y >= 1)
            {
                scale.y = 1;
                _upDown = !_upDown;
            }
            this.transform.localScale = scale;
        }
        else
        {
            Vector3 scale = this.transform.localScale;
            scale.y -= Time.deltaTime;
            if (scale.y <= 0.1f)
            {
                scale.y = 0;
                _upDown = !_upDown;
            }
            this.transform.localScale = scale;
        }
        this.transform.localPosition += -this.transform.up * _speed * Time.deltaTime;
    }
}
