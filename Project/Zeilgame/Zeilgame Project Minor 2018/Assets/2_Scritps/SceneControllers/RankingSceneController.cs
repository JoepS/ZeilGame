using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class RankingSceneController : MonoBehaviour {

    [SerializeField] Dropdown _boatDropDown;
    [SerializeField] GameObject _rankingPanelParent;
    [SerializeField] GameObject _rankingPanel;

    //False == Overall && True == Tournament
    bool _overallOrTournament = false;

    [SerializeField] Text _overallOrTournamentButtonText;

	// Use this for initialization
	void Start () {
        List<Boat> boats = MainGameController.instance.databaseController.connection.Table<Boat>().ToList();
        List<string> options = new List<string>();
        foreach(Boat b in boats)
        {
            options.Add(b.Name);
        }
        _boatDropDown.AddOptions(options);
        ShowBoatRankings(boats.First().id);
	}

	// Update is called once per frame
	void Update () {

	}

    public void ShowBoatRankings(int boatId)
    {
        for (int i = 0; i < _rankingPanelParent.transform.childCount; i++)
        {
            Destroy(_rankingPanelParent.transform.GetChild(i).gameObject);
        }

        if (!_overallOrTournament)
        {
            List<BoatRanking> boatRankings = MainGameController.instance.databaseController.connection.Table<BoatRanking>().Where(x => x.BoatId == boatId).ToList();
            boatRankings = boatRankings.OrderBy(x => x.Points).ThenBy(x => x.RacesWon).ToList();
            foreach (BoatRanking br in boatRankings)
            {
                GameObject rankingPanel = GameObject.Instantiate(_rankingPanel);
                rankingPanel.transform.SetParent(_rankingPanelParent.transform);
                rankingPanel.transform.localScale = Vector3.one;
                RankingPanel rp = rankingPanel.GetComponent<RankingPanel>();
                if (br.PersonId != MainGameController.instance.player.id)
                {
                    List<Person> temppersons = MainGameController.instance.databaseController.connection.Table<Person>().Where(x => x.id == br.PersonId).ToList();
                    Person p = temppersons.First();
                    rp.SetData(br, p);
                }
                else
                {
                    rp.SetData(br, MainGameController.instance.player);
                }
            }
        }
        else
        {
            List<Person> persons = MainGameController.instance.databaseController.connection.Table<Person>().ToList();
            persons = persons.OrderBy(x => x.LifetimePoints).ThenBy(x => x.RacesWon).Where(x => x.LifetimePoints > 0).ToList();
            foreach (Person p in persons)
            {
                GameObject rankingPanel = GameObject.Instantiate(_rankingPanel);
                rankingPanel.transform.SetParent(_rankingPanelParent.transform);
                rankingPanel.transform.localScale = Vector3.one;
                RankingPanel rp = rankingPanel.GetComponent<RankingPanel>();
                rp.SetData(p);
            }
        }
    }

    public void OnBackButtonClick()
    {
        MainGameController.instance.sceneController.OneSceneBack();
    }

    public void OnBoatDropdownValueChanged()
    {
        string boatName = _boatDropDown.options[_boatDropDown.value].text;
        Boat b = MainGameController.instance.databaseController.connection.Table<Boat>().Where(x => x.Name == boatName).First();
        ShowBoatRankings(b.id);
    }

    public void OnOverallOrTournamentButtonClick()
    {
        string boatName = _boatDropDown.options[_boatDropDown.value].text;
        Boat b = MainGameController.instance.databaseController.connection.Table<Boat>().Where(x => x.Name == boatName).First();
        _overallOrTournament = !_overallOrTournament;
        if (_overallOrTournament)
        {
            _overallOrTournamentButtonText.text = MainGameController.instance.localizationManager.GetLocalizedValue("overall_text");
            _boatDropDown.gameObject.SetActive(false);
        }
        else
        {
            _overallOrTournamentButtonText.text = MainGameController.instance.localizationManager.GetLocalizedValue("tournament_text");
            _boatDropDown.gameObject.SetActive(true);
        }
        ShowBoatRankings(b.id);
    }
}
