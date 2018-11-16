using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Responses
{
    Weather,
    WeatherRect,
    WeatherImg
}

public class NetworkController : MonoBehaviour
{
    [SerializeField] OpenWeatherMapController _owmController;
    public WeatherData lastWeatherData
    {
        get
        {
            return _owmController.lastData;
        }
    }

    [SerializeField] Dictionary<string, object> _responses;
    static int weatherRectIndex = 0;
    static int weatherIndex = 0;
    static int weatherImageIndex = 0;

    // Use this for initialization
    void Start()
    {
        _responses = new Dictionary<string, object>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void getTheWeather(Vector2 position)
    {
        string url = _owmController.getRequest(position);
        StartCoroutine(request(url, weatherIndex));
    }

    public void getWeatherInRectangle(float lonleft, float latbottom, float lonright, float lattop, int zoom)
    {
        string url = _owmController.getRectangleRequest(lonleft, latbottom, lonright, lattop, zoom);
        StartCoroutine(request(url, weatherRectIndex));
    }

    public int getWindImage(int zoom, int x, int y)
    {
        string url = _owmController.getWindImageRequest(zoom, x, y);
        weatherImageIndex++;
        StartCoroutine(request(url, weatherImageIndex));
        return weatherImageIndex;
    }

    public int GetCurrentWeatherImageIndex()
    {
        return weatherImageIndex;
    }

    public IEnumerator request(string url, int index)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.error == string.Empty || www.error == null)
        {
            if (www.texture.height == 256)
            {
                Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.DXT1, false);
                www.LoadImageIntoTexture(texture);
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                Sprite spriteToUse = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100);

                OWMWeatherImage oWMWeatherImage = new OWMWeatherImage(spriteToUse, Vector2.negativeInfinity, -1, System.DateTime.Parse(www.responseHeaders["Date"]), index);

                _responses.Add(Responses.WeatherImg.ToString() + index, oWMWeatherImage);

            }
            processResponse(www.text);
        }
        else
        {
            Debug.LogError(url + "\n gives the folowing error:" + www.error);
        }
    }

    public void processResponse(string json)
    {
        if (json.Contains("calctime"))
        {
            RectangularWeatherData rwd = JsonUtility.FromJson<RectangularWeatherData>(json);
            string key = Responses.WeatherRect.ToString() + weatherRectIndex;
            _responses.Add(key, rwd);
            weatherRectIndex++;
        }
        else if (json.Contains("coord"))
        {
            JsonData jd = JsonMapper.ToObject(json);
            WeatherData wd = new WeatherData(jd);
            _owmController.lastData = wd;
            string key = Responses.Weather.ToString() + weatherIndex;
            weatherIndex++;
            _responses.Add(key, wd);
        }
    }

    public object getResponse(Responses type, int index)
    {
        string key = "";
        if (index < 0)
        {
            switch (type)
            {
                case Responses.Weather:
                    index = weatherIndex -1;
                    break;
                case Responses.WeatherRect:
                    index = weatherRectIndex -1;
                    break;
                case Responses.WeatherImg:
                    index = weatherImageIndex-1;
                    break;
                default:
                    break;
            }
        }
        key = type.ToString() + (index);
        if (!_responses.ContainsKey(key))
        {
#if UNITY_EDITOR
            Debug.LogWarning("I dont have this key: " + key);
            Debug.LogWarning("WeatherIndex: " + (weatherIndex - 1) + "/ WeatherRectIndex: " + (weatherRectIndex - 1) + "/ WeatherImageIndex: " + (weatherImageIndex - 1));
#endif
            return null;
        }
        object value = _responses[key];
        _responses.Remove(key.ToString());
        if (type == Responses.WeatherRect)
        {
            weatherRectIndex--;
        }
        return value;
    }



}
