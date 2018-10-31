using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class AchievementSceneController : MonoBehaviour {

    [SerializeField] GameObject _achievementPanelPrefab;
    [SerializeField] GameObject _achievementViewContent;

    [SerializeField] Text _achievementPointsText;

	// Use this for initialization
	void Start () {
        ViewAchivements();
        UpdateAchievementPoints();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void UpdateAchievementPoints()
    {
        float points = 0;
        foreach (Achievement a in MainGameController.instance.databaseController.connection.Table<Achievement>())
        {
            if (a.Unlocked)
                points += a.Points;
        }
        _achievementPointsText.text = MainGameController.instance.localizationManager.GetLocalizedValue("achievement_points_text") + " " + points;
    }

    void ViewAchivements()
    {
        List<Achievement> achievements = MainGameController.instance.databaseController.connection.Table<Achievement>().Where(x => x.Hidden == false).ToList();
        foreach(Achievement a in achievements)
        {
            GameObject go = GameObject.Instantiate(_achievementPanelPrefab);
            go.transform.SetParent(_achievementViewContent.transform);
            go.transform.localScale = Vector3.one;
            go.GetComponent<AchievementPanel>().SetData(a);

        }
    }

    public void OnBackButtonClick()
    {
        MainGameController.instance.sceneController.OneSceneBack();
    }
}
