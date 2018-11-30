using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogHandler : MonoBehaviour {

    public string output = "";
    public string stack = "";

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        output = logString;
        if(type == LogType.Error || type == LogType.Exception || type == LogType.Warning)
            MainGameController.instance.popupManager.ViewPopup(output, null, 10);
        stack = stackTrace;
    }
}
