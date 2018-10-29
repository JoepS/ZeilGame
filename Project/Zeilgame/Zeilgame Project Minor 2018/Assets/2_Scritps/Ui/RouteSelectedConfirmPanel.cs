using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteSelectedConfirmPanel : MonoBehaviour {

    [SerializeField] Text _fromText;
    [SerializeField] Text _toText;
    [SerializeField] Text _distanceText;
    [SerializeField] Text _timeText;

	public void SetData(string from, string to, float distance, TimeSpan time)
    {
        _fromText.text = from;
        _toText.text = to;
        _distanceText.text = UnitConvertor.Convert(distance) + UnitConvertor.UnitString();
        string hourzero = "";
        string minutezero = "";
        string secondzero = "";
        if (time.Hours < 10)
            hourzero = "0";
        if (time.Minutes < 10)
            minutezero = "0";
        if (time.Seconds < 10)
            secondzero = "0";
        _timeText.text = time.Days + " " + MainGameController.instance.localizationManager.GetLocalizedValue("days_text") + " " + hourzero + time.Hours + ":" + minutezero + time.Minutes + ":" + secondzero + time.Seconds;
    }
}
