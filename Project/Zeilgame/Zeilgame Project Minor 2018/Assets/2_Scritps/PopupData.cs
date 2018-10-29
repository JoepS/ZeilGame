using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupData  {
    string _text;
    public string text { get { return _text; } }
    Sprite _image;
    public Sprite image { get { return _image; } }
    float _viewTime;
    public float viewTime { get { return _viewTime; } }

    GameObject _assignedToGameObject;
    public GameObject assignedToGameObject { get { return _assignedToGameObject; } set { _assignedToGameObject = value; } }


    public PopupData(string text, Sprite image, float viewTime)
    {
        _text = text;
        _image = image;
        _viewTime = viewTime;
    }
}
