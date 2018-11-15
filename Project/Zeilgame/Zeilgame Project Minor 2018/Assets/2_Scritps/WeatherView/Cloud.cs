using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour {
    public bool _moving = false;

    float _speed = 100;
    public float Speed { set { _speed = value; } }

    float _width;

    [SerializeField] GameObject _thunderImage;

    Blink _thunderBlink;

    void Start()
    {
        _width = this.GetComponent<RectTransform>().rect.width;
    }

    void Update()
    {
        if (_moving)
        {
            this.transform.localPosition -= new Vector3(_speed * Time.deltaTime, 0, 0);
            if (this.transform.localPosition.x <= -((Screen.width/2) + (_width/2)))
            {
                this.transform.localPosition = new Vector3((Screen.width/2) + (_width / 2), 0, -1);
                this.Moving(false, 0, _thunderBlink);
                //_moving = false;
            }
            Vector3 loc = _thunderImage.transform.localPosition;
            if (_thunderBlink.StartingToBlink)
                _thunderImage.transform.localPosition += new Vector3(Random.Range(-250, 250), 0, 0);
            if (_thunderImage.transform.localPosition.x > Screen.width / 2 || _thunderImage.transform.localPosition.x < -Screen.width / 2)
                _thunderImage.transform.localPosition = loc;
            _thunderImage.SetActive(_thunderBlink.StartingToBlink);
        }
    }

    public void Moving(bool value, int rainAmount, Blink thunder)
    {
        this._moving = value;
        this.gameObject.SetActive(value);
        this.GetComponent<ParticleSystem>().emissionRate = rainAmount;
        _thunderBlink = thunder;
    }
}
