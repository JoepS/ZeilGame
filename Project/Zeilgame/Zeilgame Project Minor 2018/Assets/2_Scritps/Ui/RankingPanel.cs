using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingPanel : MonoBehaviour {

    [SerializeField] Image _personImage;
    [SerializeField] Text _nameText;
    [SerializeField] Text _racesSailedText;
    [SerializeField] Text _racesWonText;
    [SerializeField] Text _pointsText;

	public void SetData(BoatRanking ranking, Person person)
    {
        _nameText.text = person.Name;
        _racesSailedText.text = "" + ranking.RacesSailed;
        _racesWonText.text = "" + ranking.RacesWon;
        _pointsText.text = MainGameController.instance.localizationManager.GetLocalizedValue("points_text") + ": \n" + ranking.Points;
    }

    public void SetData(Person person)
    {
        _nameText.text = person.Name;
        _racesSailedText.text = "" + person.RacesSailed;
        _racesWonText.text = "" + person.RacesWon;
        _pointsText.text = MainGameController.instance.localizationManager.GetLocalizedValue("points_text") + ": \n" + person.LifetimePoints;
    }

    public void SetData(BoatRanking ranking, Player player)
    {
        _nameText.text = player.Name;
        _racesSailedText.text = "" + ranking.RacesSailed;
        _racesWonText.text = "" + ranking.RacesWon;
        _pointsText.text = MainGameController.instance.localizationManager.GetLocalizedValue("points_text") + ": \n" + ranking.Points;
    }
}
