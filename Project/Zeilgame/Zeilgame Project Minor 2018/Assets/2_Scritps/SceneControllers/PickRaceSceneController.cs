using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class PickRaceSceneController : MonoBehaviour {

    const string BoatShopSceneName = "BoatShopScene";
    const string SailShopSceneName = "SailShopScene";

    [SerializeField] GameObject _racePanel;
    [SerializeField] GameObject _scrollviewContent;

    public static WeatherData CurrentWeatherData;

    [SerializeField] Text _weatherText;
    [SerializeField] Text _windSpeedText;

    [SerializeField] GameObject _noBoatPanel;
    [SerializeField] GameObject _noSailPanel;

    void GetWeather()
    {
        CurrentWeatherData = null;
        MainGameController.instance.networkController.getTheWeather(MainGameController.instance.player.getCurrentLocationLatLon());
        StartCoroutine(WaitForResponse());
    }

    IEnumerator WaitForResponse()
    {
        CurrentWeatherData = (WeatherData)MainGameController.instance.networkController.getResponse(Responses.Weather, -1);
        while (CurrentWeatherData == null)
        {
            CurrentWeatherData = (WeatherData)MainGameController.instance.networkController.getResponse(Responses.Weather, -1);
            yield return new WaitForSeconds(0.1f);
        }

        _weatherText.text = CurrentWeatherData.getWeatherText();
        _windSpeedText.text = MainGameController.instance.localizationManager.GetLocalizedValue("wind_speed_text") + ": \n" + CurrentWeatherData.getWindSpeed() + "kt";
    }

    // Use this for initialization
    void Start () {
        GetWeather();
        if (MainGameController.instance.player.GetActiveBoat() == null)
        {
            _noBoatPanel.SetActive(true);
        }
        else if(MainGameController.instance.player.GetActiveBoat().GetSailsBought().Count == 0)
        {
            _noSailPanel.SetActive(true);
        }
        else
        {
            int playerBoatId = MainGameController.instance.player.GetActiveBoat().id;
            int playerLocationId = MainGameController.instance.player.CurrentLocation;
            List<Race> races = MainGameController.instance.databaseController.connection.Table<Race>().Where(x => x.BoatsUsedId == playerBoatId && x.LocationId == playerLocationId).ToList();
            foreach (Race r in races)
            {
                CreatRacePanel(r);
            }
        }
	}

    void CreatRacePanel(Race r)
    {
        GameObject go = GameObject.Instantiate(_racePanel);
        go.GetComponent<RacePanel>().SetData(r);
        go.transform.SetParent(_scrollviewContent.transform);
        go.transform.localScale = Vector3.one;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnBackButtonClick()
    {
        MainGameController.instance.sceneController.OneSceneBack();
    }

    public void OnBuyBoatButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(BoatShopSceneName);
    }

    public void OnBuySailButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(SailShopSceneName);
    }
}
