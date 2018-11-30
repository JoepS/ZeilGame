using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class MapSceneController : MonoBehaviour {

    [SerializeField] WorldMapController _worldMapController;
    [SerializeField] GameObject _scrollViewContent;

    [SerializeField] Button _zoomInButton;
    [SerializeField] Button _zoomOutButton;

    [SerializeField] GameObject _popupTextPanel;
    [SerializeField] Text _popupText;

    int _currZoom = 1;
    int _maxZoom = 8;
    int _minZoom = 1;

	// Use this for initialization
	void Start () {
        

        AddMapLocationPointers();
	}

    public void AddMapLocationPointers()
    {
        List<MapLocationPointer> pointers = _worldMapController.GetLocationPointers();
        foreach(MapLocationPointer mlp in pointers)
        {
            mlp.GetButton().onClick.AddListener(delegate { OnCityClick(mlp); });
        }
    }

    public void ZoomMap(bool increase)
    {
        _currZoom = (increase) ? _currZoom * 2 : _currZoom / 2;
        if (_currZoom > _maxZoom)
        {
            _currZoom = _maxZoom;
            _zoomInButton.interactable = false;
            return;
        }
        else
            _zoomInButton.interactable = true;
        if (_currZoom < _minZoom)
        {
            _currZoom = _minZoom;
            _zoomOutButton.interactable = false;
            return;
        }
        else
            _zoomOutButton.interactable = true;

        float newsize = 4096 * _currZoom;
        Vector2 size = new Vector2(newsize, newsize);
        //_scrollViewContent.GetComponent<RectTransform>().sizeDelta = size;

        _worldMapController.SetZoom(_currZoom);
        _worldMapController.SetNewSize(size);

        _worldMapController.ScrollToPosition(MainGameController.instance.player.getCurrentLocationLatLon());
        _worldMapController.CreateLocationPointers(false);
        AddMapLocationPointers();
    }

	// Update is called once per frame
	void Update () {

	}

    public void OnCityClick(MapLocationPointer mlp)
    {
        _worldMapController.ScrollToPosition(new Vector2(mlp.GetLocation().lat, mlp.GetLocation().lon));

        _popupTextPanel.transform.localPosition = mlp.transform.localPosition - new Vector3(0, 255, 0);
        _popupText.text = mlp.GetLocation().Name;
        _popupTextPanel.SetActive(true);
    }

    public void OnBackButtonClick()
    {
        MainGameController.instance.sceneController.OneSceneBack();
    }
}
