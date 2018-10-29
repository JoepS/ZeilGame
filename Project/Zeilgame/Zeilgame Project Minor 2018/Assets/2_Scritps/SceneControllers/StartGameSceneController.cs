using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StartGameSceneController : MonoBehaviour {

    const string _selectLocationSceneName = "SelectLocation";
    const string _optionsSceneName = "OptionsScene";
    const string _villageSceneName = "VillageScene";
    const string SailingSceneName = "SailingScene";

    public void OnStartGameButtonClick()
    {
        if(MainGameController.instance.player != null)
        {
            if (MainGameController.instance.player.CurrentRoute > 0)
            {
                MainGameController.instance.sceneController.LoadScene(SailingSceneName);
            }
            else
                MainGameController.instance.sceneController.LoadScene(_villageSceneName);
        }
        else
            MainGameController.instance.sceneController.LoadScene(_selectLocationSceneName);
    }

    public void OnOptionsButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(_optionsSceneName);
    }

    public void OnExitGameButtonClick()
    {
        MainGameController.instance.CloseGame();
    }
}
