using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class RequestSceneController : MonoBehaviour {

    [SerializeField] GameObject _requestPanel;
    [SerializeField] GameObject _requestPanelPrefab;
    [SerializeField] Dropdown _dropDown;

    List<Request> _requests;

	// Use this for initialization
	void Start ()
    {
        _requests = MainGameController.instance.databaseController.connection.Table<Request>().Where(x => x.LocationId == MainGameController.instance.player.CurrentLocation || x.Accepted).ToList();
        CreateRequestPanels();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void CreateRequestPanels()
    {
        for(int i = 0; i < _requestPanel.transform.childCount; i++)
        {
            Destroy(_requestPanel.transform.GetChild(i).gameObject);
        }
        foreach(Request r in _requests)
        {
            GameObject go = GameObject.Instantiate(_requestPanelPrefab, _requestPanel.transform);
            go.transform.localScale = Vector3.one;
            go.GetComponent<RequestPanel>().SetData(r, delegate { OnDropDownValueChanged(); });
        }
    }

    public void OnDropDownValueChanged()
    {
        string value = _dropDown.options[_dropDown.value].text;
        _requests = MainGameController.instance.databaseController.connection.Table<Request>().Where(x => x.LocationId == MainGameController.instance.player.CurrentLocation || x.Accepted).ToList();
        switch (value)
        {
            case "Avaliable":
                _requests = _requests.Where(x => !x.Completed && !x.Accepted).ToList();
                break;
            case "Accepted":
                _requests = _requests.Where(x => !x.Completed && x.Accepted).ToList();
                break;
            case "Completed":
                _requests = _requests.Where(x => x.Completed).ToList();
                break;
            default:
                break;
        }
        CreateRequestPanels();
    }

    public void OnBackButtonClick()
    {
        MainGameController.instance.sceneController.OneSceneBack();
    }
}
