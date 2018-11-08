using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerProfileSceneController : MonoBehaviour {

    [SerializeField] Text _playerName;
    [SerializeField] Text _playerLevel;
    [SerializeField] Slider _playerExperienceTilNextLevelSlider;
    [SerializeField] Text _playerExperienceTilNextLevelText;
    [SerializeField] Text _totalExperienceText;
    [SerializeField] GameObject _propertiesContent;
    [SerializeField] GameObject _propertiesPanelPrefab;

	// Use this for initialization
	void Start () {
        Player player = MainGameController.instance.player;
        _playerName.text = player.Name;
        _playerLevel.text = player.Level + "";
        int curExperience = player.Experience;
        int nextLevelExperience = ExperienceController.GetExperienceForLevel(player.Level);
        _playerExperienceTilNextLevelSlider.value = (float)curExperience / (float)nextLevelExperience;
        _playerExperienceTilNextLevelText.text = curExperience + " / " + nextLevelExperience;
        int totalExperience = 0;
        for (int i = 0; i < player.Level - 1; i++)
            totalExperience += ExperienceController.GetExperienceForLevel(i);
        totalExperience += curExperience;
        _totalExperienceText.text = totalExperience + "";

        foreach (Property p in MainGameController.instance.databaseController.connection.Table<Property>().ToList())
            CreatePropertyPanel(p);
	}

    public void CreatePropertyPanel(Property p)
    {
        GameObject pp = GameObject.Instantiate(_propertiesPanelPrefab, _propertiesContent.transform);
        pp.transform.localScale = Vector3.one;
        pp.GetComponent<PropertyPanel>().SetData(p.Name, p.Value);

    }

    public void OnBackButtonClick()
    {
        MainGameController.instance.sceneController.OneSceneBack();
    }
}
