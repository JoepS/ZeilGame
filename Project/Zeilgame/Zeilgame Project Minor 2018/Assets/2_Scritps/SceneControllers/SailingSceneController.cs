using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class SailingSceneController : MonoBehaviour
{


    const string MapSceneName = "WeatherScene";
    const string VillageSceneName = "VillageScene";

    [SerializeField] GameObject _boatImage;
    [SerializeField] SeaMovement _seaMovement;

    [SerializeField] Text _distanceLeftText;
    [SerializeField] Text _arrivelTimeText;
    [SerializeField] Text _currentSpeedText;
    [SerializeField] Text _maxSpeedText;

    Route _route;

    float _currentSpeed;

    bool useOfflineSpeed = true;

    [SerializeField] WeatherController _weatherController;

    bool DamageCoroutineStarted = false;

    [SerializeField] GameObject _arrivedPanel;
    [SerializeField] Text _arrivalLocationText;

    bool _isMoving;

    DateTime _arrivalTime;

    [SerializeField] GameObject _sailChangePanel;
    [SerializeField] GameObject _sailChangePanelContent;
    [SerializeField] GameObject _sailChangePanelPrefab;

    List<GameObject> _sailChangePanels;

    [SerializeField] GameObject _inventoryPanel;
    [SerializeField] GameObject _inventoryPanelContent;
    [SerializeField] GameObject _inventoryItemPanelPrefab;

    List<ItemUsePanel> _itemUsePanels;

    bool _receivedOfflineDamage = false;
    TimeSpan _timeSinceLastLogin;


    private void Awake()
    {
        if (MainGameController.instance == null)
            return;
        Debug.Log("SailingSceneController.Awake()");
        MainGameController.instance.canGoBack = false;
        /*
        if (MainGameController.instance.sceneController.StackCount() > 0)
        {
            while (MainGameController.instance.sceneController.StackCount() > 0)
            {
                if (MainGameController.instance.sceneController.PeekStack().GetType() == typeof(Route))
                {
                    _route = (Route)MainGameController.instance.sceneController.PopFromStack();
                }
                else if(MainGameController.instance.sceneController.PeekStack().GetType() == typeof(bool))
                {
                    useOfflineSpeed = (bool)MainGameController.instance.sceneController.PopFromStack();
                }
                else
                {
                    Debug.LogError("Unknown object on stack: " + MainGameController.instance.sceneController.PopFromStack());
                }

            }
        }*/
    }

    // Use this for initialization
    void Start () {
        SetChangeSailsPanel();
        SetInventoryPanelItems();

        MainGameController.instance.canGoBack = false;

        _route = MainGameController.instance.databaseController.connection.Table<Route>().Where(x => x.id == MainGameController.instance.player.CurrentRoute).First();
        _timeSinceLastLogin = DateTime.Now - MainGameController.instance.player.GetLastInGame();

        Vector2 playerposition = new Vector2(MainGameController.instance.player.CurrentLocationLat, MainGameController.instance.player.CurrentLocationLon);
        Location startLocation = MainGameController.instance.databaseController.connection.Table<Location>().Where(x => x.id == MainGameController.instance.player.CurrentLocation).First();
        float totaldistance = _route.GetRouteDistanceFromPosition(playerposition, startLocation);
        float speed = MainGameController.instance.player.GetActiveBoat().GetSpeed();
        if (useOfflineSpeed)
            speed = MainGameController.instance.player.GetActiveBoat().GetOfflineSpeed();

        float distanceOffline = ((float)_timeSinceLastLogin.TotalHours * speed);
        MainGameController.instance.achievementManager.AddAchievementProperty(AchievementProperties.Distance, distanceOffline);
        float distance = totaldistance - distanceOffline;
        _route.SetDistanceLeft(distance);

        if(_route.GetDistanceLeft() <= 0)
        {
            ArrivedAtLocation();
        }
        _currentSpeed = MainGameController.instance.player.GetActiveBoat().GetSpeed();

        _isMoving = true;
        StartCoroutine(movement());
        StartCoroutine(SpeedClickMultiplier());
        if(_weatherController.GetCurrentData() != null){
            if (_weatherController.GetCurrentData().getWindSpeed() > MainGameController.instance.player.GetActiveBoat().GetMaxWindSpeedSails() && !DamageCoroutineStarted)
            {
                float minutes = (float)_timeSinceLastLogin.TotalMinutes;
                MainGameController.instance.player.GetActiveBoat().Damage += minutes * 0.01f;
                #if UNITY_EDITOR
                Debug.Log("Received " + (minutes * 0.01f) + " damage");
                #endif
                _receivedOfflineDamage = true;
                StartCoroutine(receiveDamage());
            }
        }
    }

    float SpeedMultiplier = 0;

	// Update is called once per frame
	void Update () {


        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    DayNightAffected.now = DayNightAffected.now.Add(new TimeSpan(1, 0, 0));
        //}
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    DayNightAffected.now = new DateTime(2018, 10, 16, 1, 0, 0);
        //}


        MoveBoat();
        UpdateInfoPanels();


	}

    IEnumerator receiveDamage()
    {
        while (true)
        {
            float randomDamageAmount = UnityEngine.Random.Range(0.1f, 1f);
            if (MainGameController.instance.player.GetActiveBoat().Damage + randomDamageAmount > MainGameController.instance.player.GetActiveBoat().GetMaxDamage())
            {
                MainGameController.instance.player.GetActiveBoat().Damage = MainGameController.instance.player.GetActiveBoat().GetMaxDamage();
            }
            else
            {
                MainGameController.instance.player.GetActiveBoat().Damage += randomDamageAmount;
            }
            MainGameController.instance.player.GetActiveBoat().Save();
            float waitTime = UnityEngine.Random.Range(15, 60);
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator SpeedClickMultiplier()
    {
        while (_isMoving)
        {
            _currentSpeed = Mathf.Lerp(MainGameController.instance.player.GetActiveBoat().GetSpeed(), MainGameController.instance.player.GetActiveBoat().GetSpeedModified(), SpeedMultiplier);
            if (Input.GetMouseButton(0))
            {
                SpeedMultiplier += Time.deltaTime;
                if (SpeedMultiplier >= 1)
                    SpeedMultiplier = 1;
            }

            SpeedMultiplier -= Time.deltaTime / 2;
            if (SpeedMultiplier <= 0)
                SpeedMultiplier = 0;

            yield return new WaitForSeconds(0.1f);
        }
        _currentSpeed = 0;
    }

    IEnumerator movement()
    {
        float distanceSailed = 0;
        while (_isMoving)
        {
            float distance = _currentSpeed / 3600;
            MainGameController.instance.achievementManager.AddAchievementProperty(AchievementProperties.Distance, distance);
            distanceSailed += distance;
            if (distanceSailed >= 1)
            {
                int multiple = (int)distanceSailed / 1;
                MainGameController.instance.player.AddExperience(ExperienceController.ExperiencePerKm * multiple);
                distanceSailed -= multiple;
            }
            float distanceLeft = _route.GetDistanceLeft() - distance;
            if (distanceLeft <= 0)
            {
                _route.SetDistanceLeft(0);
                ArrivedAtLocation();
            }
            else
            {
                _route.SetDistanceLeft(distanceLeft);
                Vector2 currentPosition = _route.GetPositionInRoute(MainGameController.instance.player.GetCurrentLocation());
                MainGameController.instance.player.CurrentLocationLat = currentPosition.x;
                MainGameController.instance.player.CurrentLocationLon = currentPosition.y;

            }
            if (_weatherController.GetCurrentData() != null)
            {


                if (_weatherController.GetCurrentData().getWindSpeed() > MainGameController.instance.player.GetActiveBoat().GetMaxWindSpeedSails() &&
                    !DamageCoroutineStarted)
                {
                    if (!_receivedOfflineDamage)
                    {
                        float minutes = (float)_timeSinceLastLogin.TotalMinutes;
                        MainGameController.instance.player.GetActiveBoat().Damage += minutes * 0.01f;
                        _receivedOfflineDamage = true;
                    }
                    DamageCoroutineStarted = true;
                    StartCoroutine(receiveDamage());
                }
                if(_weatherController.GetCurrentData().getWindSpeed() < MainGameController.instance.player.GetActiveBoat().GetMaxWindSpeedSails() && DamageCoroutineStarted)
                {
                    DamageCoroutineStarted = false;
                    StopCoroutine(receiveDamage());
                }
            }
            _arrivalTime = DateTime.Now + _route.GetTimeLeft(_currentSpeed);

            //float seaMovementSpeed = _currentSpeed / 2;// (int)MainGameController.Map(_currentSpeed, MainGameController.instance.player.GetActiveBoat().GetSpeed(), MainGameController.instance.player.GetActiveBoat().GetSpeedModified(), 1, 10);
            //_seaMovement.SetSpeed(seaMovementSpeed);

            yield return new WaitForSeconds(1);
        }
    }

    public void MoveBoat()
    {
        float middley = _seaMovement.GetMiddleBlockHeight();
        float middleTOy = _seaMovement.GetMiddleToOneBlockHeight();
        Vector3 pos = _boatImage.transform.localPosition;
        pos.y = middley - ((_boatImage.GetComponent<RectTransform>().sizeDelta.y / 4));// - (Screen.height / 2);
        _boatImage.transform.localPosition = pos;

        float a = 5;
        float b = middleTOy - middley;
        float c = Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));

        float angle = Mathf.Sin(b / c);
        Vector3 angles = _boatImage.transform.localEulerAngles;
        angles.z = Mathf.Rad2Deg * angle;
        _boatImage.transform.localEulerAngles = angles;

    }

    public void OnMapButtonClick()
    {
        MainGameController.instance.player.SetLastInGame(JsonUtility.ToJson((JsonDateTime)DateTime.Now));
        MainGameController.instance.player.Save();
        MainGameController.instance.sceneController.LoadScene(MapSceneName, true, LoadSceneMode.Additive);
    }

    public void UpdateInfoPanels()
    {
        LocalizationManager lz = MainGameController.instance.localizationManager;
        _distanceLeftText.text = lz.GetLocalizedValue("distance_left_text") + ":\n " + UnitConvertor.Convert(_route.GetDistanceLeft()) + UnitConvertor.UnitString();
        _arrivelTimeText.text = lz.GetLocalizedValue("excpected_arrival_time_text") + ":\n " + _arrivalTime.ToString("dd-MM-yyyy HH:mm:ss");
        _currentSpeedText.text = lz.GetLocalizedValue("current_speed_text") + ":\n " + Mathf.Round(UnitConvertor.Convert(_currentSpeed * 10)) / 10 + UnitConvertor.UnitString() + "/h";
        _maxSpeedText.text = lz.GetLocalizedValue("maximum_speed_text") + ":\n " + Mathf.Round(UnitConvertor.Convert(MainGameController.instance.player.GetActiveBoat().GetSpeedModified() * 10)) / 10 + UnitConvertor.UnitString() + "/h";
    }

    void ArrivedAtLocation()
    {
        _isMoving = false;
        Location arrivalLocation = _route.GetToLocation();
        if (MainGameController.instance.player.CurrentLocation == _route.GetToLocation().id)
        {
            arrivalLocation = _route.GetFromLocation();
        }

        _arrivalLocationText.text = arrivalLocation.Name;
        MainGameController.instance.player.CurrentLocation = arrivalLocation.id;
        MainGameController.instance.player.setCurrentLocationLatLon(arrivalLocation.GetLocation());
        MainGameController.instance.player.CurrentRoute = -1;

        RequestManager.UpdateRequest(MainGameController.instance.player.GetActiveBoat(), arrivalLocation);

        _arrivedPanel.SetActive(true);
    }

    public void EnterLocationButtonClick()
    {
        MainGameController.instance.sceneController.LoadScene(VillageSceneName);
    }

    public void SetChangeSailsPanel()
    {
        _sailChangePanels = new List<GameObject>();
        List<Sail> sails = MainGameController.instance.player.GetActiveBoat().GetSailsBought();
        foreach(Sail s in sails)
        {
            GameObject go = GameObject.Instantiate(_sailChangePanelPrefab);
            go.GetComponent<SailChangePanel>().SetData(s);
            go.GetComponent<SailChangePanel>().GetButton().onClick.AddListener(delegate { ChangeSail(s); });
            go.transform.SetParent(_sailChangePanelContent.transform);
            go.transform.localScale = Vector3.one;
            _sailChangePanels.Add(go);
        }
        _sailChangePanel.SetActive(false);

    }

    public void SetInventoryPanelItems()
    {
        if(_inventoryPanelContent.transform.childCount > 0)
        {
            for (int i = 0; i < _inventoryPanelContent.transform.childCount; i++)
            {
                Destroy(_inventoryPanelContent.transform.GetChild(i).gameObject);
            }
        }

        List<Item> items = MainGameController.instance.databaseController.connection.Table<Item>().Where(x => x.InInventory > 0).ToList();
        _itemUsePanels = new List<ItemUsePanel>();
        foreach(Item i in items)
        {
            GameObject go = GameObject.Instantiate(_inventoryItemPanelPrefab);
            go.transform.SetParent(_inventoryPanelContent.transform);
            go.transform.localScale = Vector3.one;
            go.GetComponent<ItemUsePanel>().SetData(i);
            go.GetComponent<ItemUsePanel>().GetItemUseButton().onClick.AddListener(delegate { UseItem(i); });
            _itemUsePanels.Add(go.GetComponent<ItemUsePanel>());
        }
        //_inventoryPanel.SetActive(false);
    }

    public void ChangeSailButton()
    {
        _sailChangePanel.SetActive(!_sailChangePanel.activeSelf);
    }

    public void ChangeSail(Sail sail)
    {
        Sail s = MainGameController.instance.player.GetActiveBoat().GetSailsBought().Where(x => x.Active).First();
        s.Active = false;
        s.Save();
        sail.Active = true;
        sail.Save();

        List<Sail> sails = MainGameController.instance.player.GetActiveBoat().GetSailsBought().ToList();
        for(int i = 0; i< sails.Count; i++)
        {
            _sailChangePanels[i].GetComponent<SailChangePanel>().SetData(sails[i]);
        }
    }

    public void InventoryButtonClick()
    {
        if(!_inventoryPanel.activeSelf)
            SetInventoryPanelItems();
        _inventoryPanel.SetActive(!_inventoryPanel.activeSelf);
    }

    public void UseItem(Item i)
    {
        i.InInventory -= 1;
        i.Save();
        ItemUsePanel iup = _itemUsePanels.Where(x => x.item.id == i.id).First();
        iup.SetData(i);
        i.UseItem();
        if (i.InInventory <= 0)
        {
            _itemUsePanels.Remove(iup);
            Destroy(iup.gameObject);
        }
    }
}
