using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeatherViewer : MonoBehaviour {

    [SerializeField] CloudMovement _cloudMovement;
    [SerializeField] WeatherController _weatherController;
    [SerializeField] Blink _thunderFlash;
    [SerializeField] GameObject _mistImage;

    List<WeatherSetting> _weatherSettings = new List<WeatherSetting>()
    {
        new WeatherSetting("clear sky", 0, 0, false),
        new WeatherSetting("few clouds", 1, 0, false),
        new WeatherSetting("scattered clouds", 2.5f, 0, false),
        new WeatherSetting("broken clouds", 4, 0, false),
        new WeatherSetting("shower rain", 7.5f, 250, false),
        new WeatherSetting("rain", 7.5f, 100, false),
        new WeatherSetting("thunderstorm", 7.5f, 250, false),
        new WeatherSetting("snow", 3.75f, 1, true),
        new WeatherSetting("mist", -1, 0, false),
        new WeatherSetting("fog", -1, 0, false),
        new WeatherSetting("drizzle", 7.5f, 10, false),
        new WeatherSetting("drizzle rain", 7.5f, 10, false),
        new WeatherSetting("light rain", 7.5f, 75, false),
        new WeatherSetting("light intensity drizzle rain", 7.5f, 50, false)
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
        return FindWeatherSetting(type);
    }

    WeatherSetting FindWeatherSetting(string type)
    {
        Debug.Log("Weather type: " + type);
        WeatherSetting wsetting = _weatherSettings.Where(x => x.Name.Equals(type)).FirstOrDefault();
        if (wsetting == null)
        {
            Debug.LogWarning("Unknown type of weather data: " + type);
            wsetting = _weatherSettings.First();
        }
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
        DayNightAffected.FolowTime = !_currentWeatherSetting.Name.Equals("thunderstorm");

        _mistImage.SetActive(_currentWeatherSetting.CloudFrequency < 0);
        _cloudMovement.SpawningClouds((_currentWeatherSetting.CloudFrequency > 0) ? true : false, _currentWeatherSetting.CloudFrequency, _currentWeatherSetting.AmountOfDrops, _currentWeatherSetting.Snow);
        if (_currentWeatherSetting.Name.Contains("thunder"))
        {
            _thunderFlash.StartFlashing();
        }
        else
        {
            _thunderFlash.StopFlashing();
        }
    }
}

public class WeatherSetting
{
    public string Name;
    public float CloudFrequency;
    public int AmountOfDrops;
    public bool Snow;

    public WeatherSetting(string name, float cloudFrequency, int amountOfRainDrops, bool snow)
    {
        this.Name = name;
        this.CloudFrequency = cloudFrequency;
        this.AmountOfDrops = amountOfRainDrops;
        this.Snow = snow;
    }
}
