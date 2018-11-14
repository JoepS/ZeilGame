using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class OptionsSceneController : MonoBehaviour {

    const string _startMenuScene = "StartMenu";

    [SerializeField] Button _imperialButton;
    [SerializeField] Button _metricButton;

    [SerializeField] Button _simpleButton;
    [SerializeField] Button _advancedButton;

    [SerializeField] Dropdown _languageDropDown;

    [SerializeField] GameObject _sureToResetPanel;

    void Start()
    {
        if (PlayerPrefs.GetString(UnitConvertor.UnitOptionKey).Equals("Imperial"))
        {
            _imperialButton.interactable = false;
            _metricButton.interactable = true;
        }
        else
        {
            _imperialButton.interactable = true;
            _metricButton.interactable = false;
        }
        if (PlayerPrefs.GetString(BoatMovement.SimpleOrAdvancedSettingKey).Equals("simple"))
        {
            _simpleButton.interactable = false;
            _advancedButton.interactable = true;
        }
        else
        {
            _simpleButton.interactable = true;
            _advancedButton.interactable = false;
        }
        //_dutchButton.onClick.AddListener(delegate {
        //    MainGameController.instance.localizationManager.LoadLocalizedText("Nederlands.json");
        //    PlayerPrefs.SetString(LocalizationManager.LanguageKey, "Nederlands");
        //    MainGameController.instance.sceneController.LoadScene("OptionsScene", false);
        //});
        //_englishButton.onClick.AddListener(delegate {
        //    MainGameController.instance.localizationManager.LoadLocalizedText("English.json");
        //    PlayerPrefs.SetString(LocalizationManager.LanguageKey, "English");
        //    MainGameController.instance.sceneController.LoadScene("OptionsScene", false);
        //});

        _languageDropDown.AddOptions(MainGameController.instance.localizationManager.Languages);
        _languageDropDown.value = _languageDropDown.options.IndexOf(_languageDropDown.options.Where(x => x.text.Equals(MainGameController.instance.localizationManager.CurrentLanguage)).First());
        _languageDropDown.onValueChanged.AddListener(delegate { OnDropDownValueChanged(); });

    }

    void OnDropDownValueChanged()
    {
        string language = _languageDropDown.options[_languageDropDown.value].text;
        MainGameController.instance.localizationManager.LoadLocalizedText(language + ".json");
        PlayerPrefs.SetString(LocalizationManager.LanguageKey, language);
        MainGameController.instance.sceneController.LoadScene("OptionsScene", false);
    }


    public void OnBackButtonClick()
    {
        if (!MainGameController.instance.sceneController.OneSceneBack())
        {
            MainGameController.instance.sceneController.LoadScene(_startMenuScene);
        }
    }

    public void OnUnitOptionsClick(string type)
    {
        _imperialButton.interactable = !_imperialButton.interactable;
        _metricButton.interactable = !_metricButton.interactable;
        PlayerPrefs.SetString(UnitConvertor.UnitOptionKey, type);
    }

    public void OnRaceControllButtonClick(string type)
    {
        _simpleButton.interactable = !_simpleButton.interactable;
        _advancedButton.interactable = !_advancedButton.interactable;
        PlayerPrefs.SetString(BoatMovement.SimpleOrAdvancedSettingKey, type);
    }

    public void OnResetGameButtonClick()
    {
        _sureToResetPanel.SetActive(!_sureToResetPanel.activeSelf);
    }

    public void OnConfirmResetButtonClick()
    {
        //Remove player
        if(MainGameController.instance.player == null)
        {
            return;
        }
        //Remove all rankings
        foreach (BoatRanking br in MainGameController.instance.databaseController.connection.Table<BoatRanking>().ToList())
        {
            br.Delete();
        }
        //Reset boats
        foreach(Boat b in MainGameController.instance.databaseController.connection.Table<Boat>().ToList())
        {
            b.Reset();
        }
        //Reset upgrades
        foreach(Upgrade u in MainGameController.instance.databaseController.connection.Table<Upgrade>().ToList())
        {
            u.Reset();
        }
        //Reset sails
        foreach (Sail s in MainGameController.instance.databaseController.connection.Table<Sail>().ToList())
        {
            s.Reset();
        }
        //Reset items
        foreach (Item i in MainGameController.instance.databaseController.connection.Table<Item>().ToList())
        {
            i.Reset();
        }
        //Reset persons
        foreach (Person p in MainGameController.instance.databaseController.connection.Table<Person>().ToList())
        {
            if (p.id == MainGameController.instance.player.id)
                p.Delete();
            else
                p.Reset();
        }
        //Reset Achievements
        foreach(Achievement a in MainGameController.instance.databaseController.connection.Table<Achievement>().ToList())
        {
            a.Reset();
        }
        //Reset AchievementProperties
        foreach(Property ap in MainGameController.instance.databaseController.connection.Table<Property>().ToList())
        {
            ap.Reset();
        }
        //Reset Request
        foreach(Request r in MainGameController.instance.databaseController.connection.Table<Request>().ToList())
        {
            r.Reset();
        }
        MainGameController.instance.player.Delete();
        MainGameController.instance.player = null;
        MainGameController.instance.sceneController.ResetStack();
        MainGameController.instance.sceneController.LoadScene("StartMenu", false);
        MainGameController.instance.achievementManager.Reset();
    }

}
