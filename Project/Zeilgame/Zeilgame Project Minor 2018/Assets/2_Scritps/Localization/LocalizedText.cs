using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    Text text;
    public string key;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        if (text == null)
            text = GetComponentInChildren<Text>();
        text.text = MainGameController.instance.localizationManager.GetLocalizedValue(key);
        text.resizeTextForBestFit = true;
    }
}