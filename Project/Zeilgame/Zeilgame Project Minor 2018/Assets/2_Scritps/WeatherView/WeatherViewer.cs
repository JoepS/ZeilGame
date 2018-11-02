using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeatherViewer : MonoBehaviour {

    [SerializeField] CloudMovement _cloudMovement;
    [SerializeField] WeatherController _weatherController;

    List<WeatherSetting> _weatherSettings = new List<WeatherSetting>()
    {
        new WeatherSetting("clear sky", 0, 0),
        new WeatherSetting("few clouds", 1, 0),
        new WeatherSetting("scattered clouds", 2.5f, 0),
        new WeatherSetting("broken clouds", 5, 0),
        new WeatherSetting("shower rain", 7.5f, 25),
        new WeatherSetting("rain", 7.5f, 12),
        new WeatherSetting("thunderstorm", 7.5f, 25),
        new WeatherSetting("snow", 0, 0),
        new WeatherSetting("mist", 0, 0),
        new WeatherSetting("drizzle", 7.5f, 10),
        new WeatherSetting("light rain", 7.5f, 15)
    };

    WeatherData _weatherData;

    WeatherSetting _currentWeatherSetting;

	// Use this for initialization
	void Start () {
        StartCoroutine(WaitForWeatherData());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    WeatherSetting FindWeatherSetting(WeatherData data)
    {
        string type = data.getWeatherText();
        Debug.Log("Weather type: " + type);
        WeatherSetting wsetting = _weatherSettings.Where(x => x.Name.Equals(type)).FirstOrDefault();
        if (wsetting == null)
            Debug.LogWarning("Unknown type of weather data: " + type);
        return wsetting;
    }

    IEnumerator WaitForWeatherData()
    {
        _weatherData = _weatherController.GetCurrentData();
        while(_weatherData == null)
        {
            yield return new WaitForSeconds(1);
            _weatherData = _weatherController.GetCurrentData();
        }
        _currentWeatherSetting = FindWeatherSetting(_weatherData);

        _cloudMovement.SpawningClouds((_currentWeatherSetting.CloudFrequency >= 7.5) ? true : false, _currentWeatherSetting.CloudFrequency);
        RainViewer.AmountOfDrops = _currentWeatherSetting.AmountOfDrops;
    }
}

public class WeatherSetting
{
    public string Name;
    public float CloudFrequency;
    public int AmountOfDrops;

    public WeatherSetting(string name, float cloudFrequency, int amountOfRainDrops)
    {
        this.Name = name;
        this.CloudFrequency = cloudFrequency;
        this.AmountOfDrops = amountOfRainDrops;
    }
}
