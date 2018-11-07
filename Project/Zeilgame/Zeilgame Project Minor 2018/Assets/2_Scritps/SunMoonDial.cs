using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SunMoonDial : MonoBehaviour {

    DateTime now = new DateTime(2018, 10, 16, 0, 0, 0);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    now = now.Add(new TimeSpan(1, 0, 0));
        //}
        if (DayNightAffected._sunrise != 0 && DayNightAffected._sunset != 0)
        {
            now = DateTime.Now;
            Int32 unixTimestamp = (Int32)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            float angle = MainGameController.Map(unixTimestamp, DayNightAffected._sunrise, DayNightAffected._sunset, 90, 270);
            this.transform.localEulerAngles = Vector3.Lerp(this.transform.localEulerAngles, new Vector3(0, 0, angle), Time.deltaTime);
        }
	}
}
