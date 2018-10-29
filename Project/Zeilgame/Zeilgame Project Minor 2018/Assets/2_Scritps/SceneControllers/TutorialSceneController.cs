using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSceneController : MonoBehaviour {

    const string VillageSceneName = "VillageScene";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnNextButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(VillageSceneName, false);
    }
}
