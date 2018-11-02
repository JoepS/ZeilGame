using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour {
    public bool _moving = false;

    float _speed = 100;

    float _width;

    void Start()
    {
        _width = this.GetComponent<RectTransform>().rect.width;
    }

    void Update()
    {
        if (_moving)
        {
            this.transform.localPosition += new Vector3(_speed * Time.deltaTime, 0, 0);
            if (this.transform.localPosition.x >= (Screen.width/2) + (_width/2))
            {
                //this.transform.localPosition = new Vector3(-(Screen.width/2) - _width, 0, 0);
                //this.Moving(false);
                _moving = false;
            }
        }
    }

    public void Moving(bool value)
    {
        this._moving = value;
        this.gameObject.SetActive(value);
    }
}
