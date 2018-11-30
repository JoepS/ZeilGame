using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PickRouteSceneController : MonoBehaviour {

    const string SailingSceneName = "SailingScene";
    const string BoatShopSceneName = "BoatShopScene";
    const string SailShopSceneName = "SailShopScene";

    [SerializeField] GameObject _worldMap;

    [SerializeField] RouteSelectedConfirmPanel _routeSelectedConfirmPanel;

    [SerializeField] Button _zoominButton;
    [SerializeField] Button _zoomOutButton;

    int _currZoom = 1;
    int _maxZoom = 8;
    int _minZoom = 1;

    Route _selectedRoute;

    [SerializeField] Text _routeText;

    [SerializeField] GameObject _noBoatPanel;
    [SerializeField] GameObject _noSailPanel;
    [SerializeField] Button _chooseRouteButton;

	// Use this for initialization
	void Start () {
        _worldMap.GetComponent<WorldMapController>().CreateLocationPointers(true);
        addLocationListeners();
    }

	// Update is called once per frame
	void Update () {

	}

    public void ZoomMap(bool increase)
    {

        _currZoom = (increase) ? _currZoom * 2 : _currZoom / 2;
        if (_currZoom > _maxZoom)
        {
            _currZoom = _maxZoom;
            _zoominButton.interactable = false;
            return;
        }
        else
            _zoominButton.interactable = true;
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
        _worldMap.GetComponent<WorldMapController>().SetZoom(_currZoom);
        _worldMap.GetComponent<WorldMapController>().SetNewSize(size);

        _worldMap.GetComponent<WorldMapController>().ScrollToPosition(MainGameController.instance.player.getCurrentLocationLatLon());
        _worldMap.GetComponent<WorldMapController>().CreateLocationPointers(true);
        addLocationListeners();

    }

    public void addLocationListeners()
    {
        List<MapLocationPointer> _locationPointers = _worldMap.GetComponent<WorldMapController>().GetLocationPointers();
        foreach (MapLocationPointer mlp in _locationPointers)
        {
            mlp.GetButton().onClick.AddListener(delegate { OnLocationClick(mlp); });
        }
    }

    public void OnLocationClick(MapLocationPointer mlp)
    {
        Player p = MainGameController.instance.player;
        Location curLocation = MainGameController.instance.databaseController.connection.Table<Location>().Where(x => x.id == p.CurrentLocation).First();
        int destId = mlp.GetLocation().id;
        SQLite4Unity3d.TableQuery<Route> routes = MainGameController.instance.databaseController.connection.Table<Route>().Where(x =>
            (x.to == destId && x.from == curLocation.id) || (x.from == destId && x.to == curLocation.id));
        if (routes.Count() == 1)
        {
            Route route = routes.First();
            _routeText.text = curLocation.Name + " " + MainGameController.instance.localizationManager.GetLocalizedValue("to_text") + " " + mlp.GetLocation().Name;
            _worldMap.GetComponent<WorldMapController>().ShowRoute(route);
            _selectedRoute = route;

            if (MainGameController.instance.player.GetActiveBoat() == null)
            {
                _noBoatPanel.SetActive(true);
                _chooseRouteButton.interactable = false;
            }
            else if (MainGameController.instance.player.GetActiveBoat().GetSailsBought().Count == 0)
            {
                _noSailPanel.SetActive(true);
                _chooseRouteButton.interactable = false;
            }
        }
    }

    public void OnChooseRouteButtonClick()
    {
        if (_selectedRoute != null) {
            float distance = WorldMapController.calculateRouteDistanceKM(_selectedRoute);
            _selectedRoute.SetDistanceLeft(distance);

            TimeSpan timeSpan = _selectedRoute.GetTimeLeft(MainGameController.instance.player.GetActiveBoat().GetOfflineSpeed());

            _routeSelectedConfirmPanel.SetData(_selectedRoute.GetFromLocation().Name, _selectedRoute.GetToLocation().Name, distance, timeSpan);
            _routeSelectedConfirmPanel.gameObject.SetActive(true);
        }
    }

    public void OnCancelRouteSelectedButtonClick()
    {
        _routeSelectedConfirmPanel.gameObject.SetActive(false);
    }

    public void OnConfirmRouteSelectedButtonClick()
    {
        MainGameController.instance.sceneController.PushToStack(_selectedRoute);
        MainGameController.instance.player.CurrentRoute = _selectedRoute.id;
        MainGameController.instance.player.LastInGame = JsonUtility.ToJson((JsonDateTime)DateTime.Now);
        MainGameController.instance.player.Save();
        MainGameController.instance.sceneController.LoadScene(SailingSceneName);
    }

    public void OnBackButtonClick()
    {
        MainGameController.instance.sceneController.OneSceneBack();
    }

    public void OnOkButtonClick()
    {
        _noBoatPanel.SetActive(false);
        _noSailPanel.SetActive(false);
    }

    public void OnBuyBoatButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(BoatShopSceneName);
    }

    public void OnBuySailButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(SailShopSceneName);
    }
}
