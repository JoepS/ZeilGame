using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WeatherController : MonoBehaviour {

    [SerializeField] Text _weatherText;
    [SerializeField] Text _lastUpdateText;
    [SerializeField] Text _windDirectionText;
    [SerializeField] Text _windSpeedText;
    WeatherData _currentData;

	// Use this for initialization
	void Start () {
        StartCoroutine(GetWeather());   
	}

    public WeatherData GetCurrentData()
    {
        return _currentData;
    }

    IEnumerator GetWeather()
    {
        while (true)
        {
            _currentData = null;
            MainGameController.instance.networkController.getTheWeather(MainGameController.instance.player.getCurrentLocationLatLon());
            StartCoroutine(WaitForResponse());
            yield return new WaitForSeconds(3600);
        }
    }

    IEnumerator WaitForResponse()
    {
        _currentData = (WeatherData)MainGameController.instance.networkController.getResponse(Responses.Weather, -1);
        while (_currentData == null)
        {
            _currentData = (WeatherData)MainGameController.instance.networkController.getResponse(Responses.Weather, -1);
            yield return new WaitForSeconds(1);
        }
        DayNightAffected._sunrise = (float)_currentData.getSys().sunrise;
        DayNightAffected._sunset = (float)_currentData.getSys().sunset;
        showData();
    }

    public void OnWeatherUpdateClicked()
    {
        MainGameController.instance.networkController.getTheWeather(MainGameController.instance.player.getCurrentLocationLatLon());
        StartCoroutine(WaitForResponse());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void showData()
    {
        string s = _currentData.getWeatherText().First().ToString().ToUpper() + _currentData.getWeatherText().Substring(1);
        _weatherText.text = s;
        _lastUpdateText.text = MainGameController.instance.localizationManager.GetLocalizedValue("last_update_text") + ": " + _currentData.getUpdateTime().ToString("dd-MM-yy HH:mm:ss");
        _windDirectionText.text = "" + Mathf.Round((float)_currentData.getWindDir() * 10f)/10f + "°";
        _windSpeedText.text = "" + _currentData.getWindSpeed() + "kts";
    }
}
