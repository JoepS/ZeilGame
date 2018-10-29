using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RacePanel : MonoBehaviour {

    const string RaceSceneName = "RaceScene";

    [SerializeField] Text _raceNameText;
    [SerializeField] Text _boatsUsedText;
    [SerializeField] Text _difficultyText;
    [SerializeField] Image _difficultyImage;

    Race _race;

    public void SetData(Race race)
    {
        _race = race;
        _raceNameText.text = race.Name;
        _boatsUsedText.text = MainGameController.instance.localizationManager.GetLocalizedValue("boat_text") + ": " + MainGameController.instance.databaseController.connection.Table<Boat>().Where(x => x.id == race.BoatsUsedId).First().Name;
        _difficultyText.text = "" + race.Difficulty;
        _difficultyImage.color = GetDifficultyColor(race.Difficulty);
    }

    Color GetDifficultyColor(float difficulty)
    {
        Color c = Color.Lerp(Color.green, Color.red, (difficulty / 10f));
        return c;
    }

    public void OnRaceClick()
    {
        RaceSceneController.SetRace(_race);
        MainGameController.instance.sceneController.LoadScene(RaceSceneName);
    }
}
