using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageSceneController : MonoBehaviour {

    const string BoatShopSceneName = "BoatShopScene";
    const string SailShopSceneName = "SailShopScene";
    const string UpgradeShopSceneName = "UpgradeShopScene";
    const string MapSceneName = "MapScene";
    const string WeatherSceneName = "WeatherScene";
    const string PickRouteSceneName = "PickRouteScene";
    const string SailingRouteSceneName = "SailingScene";
    const string RepairsSceneName = "RepairsScene";
    const string PickRaceSceneName = "PickRaceScene";
    const string OptionsSceneName = "OptionsScene";
    const string RankingsSceneName = "RankingsScene";
    const string ItemShopSceneName = "ItemShopScene";
    const string AchievementsSceneName = "AchievementScene";
    const string PlayerProfileSceneName = "PlayerProfileScene";
    const string RequestSceneName = "RequestScene";

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void OnBoatShopButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(BoatShopSceneName);
    }

    public void OnSailShopButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(SailShopSceneName);
    }

    public void OnUpgradeShopButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(UpgradeShopSceneName);
    }

    public void OnMapButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(MapSceneName);
    }

    public void OnWeatherButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(WeatherSceneName);
    }

    public void OnPickRouteButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(PickRouteSceneName);
    }

    public void OnRaceButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(PickRaceSceneName);
    }

    public void OnRepairsButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(RepairsSceneName);
    }

    public void OnOptionsButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(OptionsSceneName);
    }

    public void OnRankingsButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(RankingsSceneName);
    }

    public void OnItemShopButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(ItemShopSceneName);
    }

    public void OnAchivementsButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(AchievementsSceneName);
    }

    public void OnPlayerProfileButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(PlayerProfileSceneName);
    }

    public void OnRequestButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(RequestSceneName);
    }
}
