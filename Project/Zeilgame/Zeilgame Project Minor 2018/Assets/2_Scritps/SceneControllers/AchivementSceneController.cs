using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AchivementSceneController : MonoBehaviour {

    [SerializeField] GameObject _achievementPanelPrefab;
    [SerializeField] GameObject _achievementViewContent;

	// Use this for initialization
	void Start () {
        ViewAchivements();
	}
	
	// Update is called once per frame
	void Update () {
		
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
