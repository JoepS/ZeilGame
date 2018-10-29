using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectLocationSceneController : MonoBehaviour {

    const string TutorialSceneName = "TutorialScene";
    const int BeginningGold = 20;

    [SerializeField] InputField _nameInputField;
    [SerializeField] Dropdown _locationDropdown;

    [SerializeField] WorldMapController _worldMapController;

    List<Location> _locations;

	// Use this for initialization
	void Start () {
        _locations = MainGameController.instance.databaseController.connection.Table<Location>().ToList();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (Location l in _locations)
        {
            options.Add(new Dropdown.OptionData(l.Name));
        }
        _locationDropdown.AddOptions(options);
        OnDropDownChanged();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnNextButtonClick()
    {
        string name = _nameInputField.text;
        Location location = _locations[_locationDropdown.value];
        Player p = MainGameController.instance.player;
        Person person = new Person();
        person.New();
        Debug.Log(p);
        if(p == null)
        {
            p = new Player();
            p.New();
            
        }
        p.Name = name;
        p.StartLocation = location.Name;
        p.CurrentLocation = location.id;
        p.CurrentLocationLat = location.lat;
        p.CurrentLocationLon = location.lon;
        p.Gold = BeginningGold;
        p.LastInGame = JsonUtility.ToJson((JsonDateTime)DateTime.Now);
        Debug.Log("Save player: " + p);
        p.Save();
        
        person.Name = name;
        person.LocationId = location.id;
        person.OpponentSetupId = -1;
        //person.BoatId = MainGameController.instance.databaseController.connection.Table<Boat>().Where(x => x.Active).First().id;
        person.Save();

        MainGameController.instance.player = p;
        MainGameController.instance.sceneController.LoadScene(TutorialSceneName, false);
    }

    public void OnDropDownChanged()
    {
        Location l = _locations[_locationDropdown.value];
        _worldMapController.ScrollToPosition(new Vector2(l.lat, l.lon));
    }
}
