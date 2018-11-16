using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWMWeatherImage {

    Sprite _image;
    public Sprite Image { get { return _image; } }
    Vector2 _position;
    public Vector2 Position { get { return _position; } set { _position = value; } }
    int _zoomLevel;
    public int ZoomLevel { get { return _zoomLevel; } set { _zoomLevel = value; } }
    System.DateTime _downloadTime;
    public System.DateTime DownloadTime { get { return _downloadTime; } }
    int _index;
    public int Index { get { return _index; } set { _index = value; } }

    public OWMWeatherImage(Sprite image, Vector2 position, int zoomLevel, System.DateTime downloadTime, int index)
    {
        _image = image;
        _position = position;
        _zoomLevel = zoomLevel;
        _downloadTime = downloadTime;
        _index = index;
    }
}
