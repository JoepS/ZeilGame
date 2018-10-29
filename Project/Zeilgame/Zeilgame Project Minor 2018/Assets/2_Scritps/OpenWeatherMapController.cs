using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWeatherMapController : MonoBehaviour
{

    const string _apiKey = "9e2182473a5d5503068d06019ac78acb";

    const string _coordinateUrl = "http://api.openweathermap.org/data/2.5/weather?"; // + lat=35&lon=139&appid=APIKEY;  
    const string _boxUrl = "http://api.openweathermap.org/data/2.5/box/city?bbox="; //12,32,15,37,10&appid=APIKEY; 
    const string _windImagesUrl = "http://tile.openweathermap.org/map/wind_new/"; //{z}/{x}/{y}.png?appid={api_key};
    const string _newWindImagesUrl = "http://tile.maps.owm.io:8099/5835780bf77ebe01008ef63d/";//{z}/{x}/{y}?
    const string _newWindImagesUrlHash = "?hash=366f03287c046a67bd8ca383af37fef6";

    WeatherData _lastData;
    public WeatherData lastData
    {
        get
        {
            return _lastData;
        }
        set
        {
            _lastData = value;
        }
    }


    public string getRequest(Vector2 position)
    {
        string url = _coordinateUrl + "lat=" + position.x + "&lon=" + position.y + "&appid=" + _apiKey;
        return url;
    }

    public string getRectangleRequest(float lonleft, float latbottom, float lonright, float lattop, int zoom)
    {
        string url = _boxUrl + lonleft + "," + latbottom + "," + lonright + "," + lattop + "," + zoom + "&appid=" + _apiKey;
        return url;
    }

    public string getWindImageRequest(int zoom, int x, int y)
    {
        string url = _newWindImagesUrl + zoom + "/" + x + "/" + y + _newWindImagesUrlHash;//".png?appid=" + _apiKey;
        return url;
    }
}
