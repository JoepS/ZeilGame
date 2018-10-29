using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightAffected : MonoBehaviour {

    Image _image;

    public static float _sunset;
    public static float _sunrise;


    //[SerializeField] public static 
    DateTime now = new DateTime(2018, 10, 16, 1, 0, 0);

    // Use this for initialization
    void Start () {
        _image = this.GetComponent<Image>();
        //now = new DateTime(2018, 10, 16, 1, 0, 0);
    }
	
	// Update is called once per frame
	void Update () {
        now = DateTime.Now;
        Int32 unixTimestamp = (Int32)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        float val = 0;
        if (now.Hour < 12)
            val = MainGameController.MapClamped(unixTimestamp, _sunrise, (_sunrise + _sunset) / 2, 0.5f, 1);
        else
            val = MainGameController.MapClamped(unixTimestamp, (_sunrise + _sunset) / 2, _sunset, 1, 0.5f);
        float h;
        float s;
        float v;
        Color.RGBToHSV(_image.color, out h, out s, out v);

        v = val;
        _image.color = Color.HSVToRGB(h, s, v);
        //_sunrise = 0.5
        //_sunset = 0.5
	}
}
