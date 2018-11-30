using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RaceSceneController : MonoBehaviour {

    public static RaceSceneController instance;

    static Race _currentRace;

#if UNITY_EDITOR
    [SerializeField] GameObject TempWaypointsPanel;
#endif

    [SerializeField] GameObject _waypointsPanel;
    [SerializeField] GameObject _waypoint;
    [SerializeField] GameObject _targetPointer;
    [SerializeField] BoatMovement _playerBoat;


    GameObject _playerTargetBuoy;

    List<GameObject> _buoys;

    public int _buoyIndicator;

    Track _currentTrack;

    bool _finishedRace = false;

    [SerializeField] GameObject _countDownPanel;
    [SerializeField] Text _countDownText;

    [SerializeField] GameObject _raceFinishedPanel;
    [SerializeField] Text _finishPlaceText;
    [SerializeField] Text _goldReceivedText;

    [SerializeField] Text _placeText;
    [SerializeField] Text _totalBoatsText;

    float _timer;
    [SerializeField] Text _timeText;

    [SerializeField] Text _windDirectionText;
    [SerializeField] Text _windSpeedText;

    public float windAngle { get { return _windAngle; } }
    [SerializeField, Range(0, 360)] float _windAngle;
    public float windSpeed { get { return _windSpeed; } }
    [SerializeField, Range(0, 50)] float _windSpeed;

    [SerializeField] GameObject _opponentsParent;
    [SerializeField] GameObject _opponentPrefab;
    List<OpponentRaceAi> _opponenents = new List<OpponentRaceAi>();

    [SerializeField] RaceMinimapController _minmapController;

    [SerializeField] GameObject _finishedRaceTarget;

    [SerializeField] List<IBoat> _finishOrder = new List<IBoat>();

    bool _pointsAwarded = false;

    List<Vector3> _opponentCurrentBuoys = new List<Vector3>();

    //StartCoroutine(GetWeather());

    WeatherData _weatherData;

    [SerializeField] GameObject _windIndicator;

    void GetWeather()
    {
        _weatherData = PickRaceSceneController.CurrentWeatherData;
        if (_weatherData != null)
        {
            //_windAngle = weatherData.getWindDir();
            _windSpeed = (float)_weatherData.getWindSpeed();
        }
    }


// Use this for initialization
void Start () {
        GetWeather();
        instance = this;
        _buoys = new List<GameObject>();
        MainGameController.instance.canGoBack = false;

        _currentTrack = MainGameController.instance.databaseController.connection.Table<Track>().Where(x => x.id == _currentRace.TrackId).First();
        DrawTrack(_currentTrack);

        //_waypointsPanel.transform.localPosition = -(Vector3.Lerp(_currentTrack.GetWaypoints()[0], _currentTrack.GetWaypoints()[1], 0.5f) - new Vector3(0, 500, 0));

        _buoyIndicator = 0;

        List<Person> _avaliablePersons = MainGameController.instance.databaseController.connection.Table<Person>().Where(x =>

            x.LocationId == _currentRace.LocationId &&
            x.BoatId == _currentRace.BoatsUsedId &&
            x.OpponentSetupId != -1

        ).ToList();

        int playerPosition = Random.Range(0, _avaliablePersons.Count);

        for (int i = 0; i < _avaliablePersons.Count + 1; i++)
        {
            if (i != playerPosition)
            {
                int newi = i;
                if (i > playerPosition)
                    newi -= 1;
                Person p = _avaliablePersons[newi];
                GameObject opponent = GameObject.Instantiate(_opponentPrefab);
                opponent.name = "Opponent " + p.Name;
                opponent.transform.SetParent(_opponentsParent.transform);
                opponent.transform.localScale = Vector3.one;
                opponent.transform.localPosition = Vector3.zero;
                OpponentRaceAi ora = opponent.GetComponentInChildren<OpponentRaceAi>();

                Boat b = MainGameController.instance.databaseController.connection.Table<Boat>().Where(x => x.id == p.BoatId).First();

                ora.gameObject.transform.position = _buoys[0].transform.position + new Vector3((i + 2) * opponent.GetComponent<RectTransform>().sizeDelta.x * 1.5f, -500);

                OpponentSetup os = MainGameController.instance.databaseController.connection.Table<OpponentSetup>().Where(x => x.id == p.OpponentSetupId).First();
                ora.SetOpponentValues(p, os.TackingAccuracy, os.TackingExtra, os.TackingSpeed, b.GetNGZLimit(), os.BestUpwindAngle);
                _opponenents.Add(ora);

                _minmapController.AddOpponent(opponent);
            }
            else
            {
                Vector3 pos = -(_currentTrack.GetWaypoints()[0] + new Vector2((i + 2) * _playerBoat.GetComponent<RectTransform>().sizeDelta.x * 1.5f, -500));
                _waypointsPanel.transform.localPosition = pos;
            }
        }
        WaypointBuoy newBuoy = _buoys[_currentTrack.GetWaypointOrder()[_buoyIndicator]].GetComponent<WaypointBuoy>();
        if (newBuoy.transform.position.y > _playerBoat.transform.position.y)
            _playerBoat.target = newBuoy.upwindTarget;
        else
            _playerBoat.target = newBuoy.downwindTarget;
        _playerTargetBuoy = _playerBoat.target;

        if(_windSpeed > _playerBoat.GetMaxWindSpeedSails())
        {
            StartCoroutine(_playerBoat.ReceiveDamage());
        }
        foreach (OpponentRaceAi ora in _opponenents)
        {
            ora.SetFinishTarget(_finishedRaceTarget);
            ora.buoys = _buoys;
            ora.trackOrder = _currentTrack.GetWaypointOrder();
            if (newBuoy.transform.position.y > ora.transform.position.y)
                ora.target = newBuoy.upwindTarget;
            else
                ora.target = newBuoy.downwindTarget;
        }

        //Vector3 euler = Vector3.zero;
        //euler.z = _windAngle;
        //_waypointsPanel.transform.localEulerAngles = euler;

        _minmapController.DrawTrack(_currentTrack);

        StartCoroutine(CountDown(5));

#if UNITY_EDITOR
        //GetWayPointsTemp();
#endif
    }

	// Update is called once per frame
	void Update () {
        //_windAngle = _waypointsPanel.transform.localEulerAngles.z;
        _targetPointer.transform.right = _playerTargetBuoy.transform.position - _targetPointer.transform.position;
        _windIndicator.transform.localRotation = Quaternion.Euler(0, 0, _windAngle);
        Vector2 pos = _waypointsPanel.transform.localPosition;
        pos -= _playerBoat.velocity;
        _waypointsPanel.transform.localPosition = pos;

        if(!_finishedRace)
            UpdatePlayerBuoys();
        else
        {
            if(_finishOrder.Count == _opponenents.Count + 1 && !_pointsAwarded)
            {
                AwardPoints();
            }
        }

        UpdateRaceUi();
    }

    IEnumerator VariableWind()
    {
        bool shrinking = false;
        float originalAngle = _windAngle;
        float change = 1f;
        while (true)
        {
            if (shrinking)
                _windAngle += change * Time.deltaTime;
            else
                _windAngle -= change * Time.deltaTime;

            if (Mathf.Abs(_windAngle - originalAngle) >= 10)
                shrinking = !shrinking;

            yield return new WaitForSeconds(change * Time.deltaTime);
        }
    }

    public void UpdatePlayerBuoys()
    {
        bool passingLine = false;
        if (_buoyIndicator < 2 || _buoyIndicator >= _currentTrack.GetWaypointOrder().Count - 2)
        {
            passingLine = _playerBoat.CheckIfPassedThroughLineBetween(_buoys[_currentTrack.GetWaypointOrder()[_buoyIndicator]],
                                                                             _buoys[_currentTrack.GetWaypointOrder()[_buoyIndicator + 1]]);
        }

        if ((_playerBoat.CheckIfBuoyPassed() && _buoyIndicator > 1 && _buoyIndicator < _currentTrack.GetWaypointOrder().Count - 2) ||
            passingLine)
        {
            if (passingLine)
                _buoyIndicator++;
            _buoyIndicator++;
            if (_buoyIndicator > _currentTrack.GetWaypointOrder().Count - 2)
            {
                _raceFinishedPanel.SetActive(true);

                _playerBoat.canMove = false;
                _finishedRace = true;
                _finishOrder.Add(_playerBoat);
                _finishPlaceText.text = "" + _finishOrder.Count;
                int gold = (_opponenents.Count - _finishOrder.Count + 2) * 10 ;
                _goldReceivedText.text = string.Format(MainGameController.instance.localizationManager.GetLocalizedValue("finished_race_gold_text"), gold);
                MainGameController.instance.player.GiveGold(gold);
            }
            else
            {
                WaypointBuoy newBuoy = _buoys[_currentTrack.GetWaypointOrder()[_buoyIndicator]].GetComponent<WaypointBuoy>();
                if (newBuoy.transform.position.y > _playerBoat.transform.position.y)
                    _playerBoat.target = newBuoy.upwindTarget;
                else
                    _playerBoat.target = newBuoy.downwindTarget;
                _playerTargetBuoy = _playerBoat.target;
            }
        }
    }

#if UNITY_EDITOR
    public void GetWayPointsTemp()
    {
        List<Vector2> list = new List<Vector2>();
        for (int i = 0; i < TempWaypointsPanel.transform.childCount; i++)
        {
            if (TempWaypointsPanel.transform.GetChild(i).gameObject.activeSelf)
            {
                list.Add(TempWaypointsPanel.transform.GetChild(i).localPosition);
            }
        }
        ListV2 v2 = new ListV2();
        v2.list = list;
    }
#endif

    public void DrawTrack(Track t)
    {
        List<Vector2> waypoints = t.GetWaypoints();

        foreach(Vector2 v2 in waypoints)
        {
            GameObject waypoint = GameObject.Instantiate(_waypoint);
            waypoint.transform.SetParent(_waypointsPanel.transform);
            waypoint.transform.localScale = Vector3.one;
            waypoint.transform.localPosition = v2;
            _buoys.Add(waypoint);
        }
    }

    public static void SetRace(Race r)
    {
        _currentRace = r;
    }

    public IEnumerator CountDown(float startTime)
    {
        while(startTime > 0)
        {
            startTime -= Time.deltaTime;
            int time = Mathf.RoundToInt(startTime);
            _countDownText.text = time + "...";
            yield return new WaitForEndOfFrame();
        }
        _countDownPanel.SetActive(false);
        _playerBoat.canMove = true;
        Wave.CanMove = true;
        foreach(OpponentRaceAi ora in _opponenents)
        {
            ora.canMove = true;
        }

        ///StartCoroutine(VariableWind());
        StartCoroutine(Timer());
    }

    public IEnumerator Timer()
    {
        while (!_finishedRace)
        {
            _timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public void UpdateRaceUi()
    {
        int minutes = (int)_timer / 60;
        float seconds = Mathf.RoundToInt(_timer - (minutes*60));

        _timeText.text = minutes + ": " + seconds;
        _windDirectionText.text = Mathf.RoundToInt(windAngle) + "°";
        _windSpeedText.text = windSpeed + " kts";

        _placeText.text = "" + CurrentPlayerPlace();
        _totalBoatsText.text = "" + (_opponenents.Count + 1);
    }

    public void OpponentFinished(OpponentRaceAi opponent)
    {
        _finishOrder.Add(opponent);
    }

    public int CurrentPlayerPlace()
    {
        _opponentCurrentBuoys = new List<Vector3>();
        for(int i = 0; i < _opponenents.Count; i++)
        {
            OpponentRaceAi ora = _opponenents[i];
            Vector3 temp = Vector3.zero;
            temp.x = ora.targetCounter;
            temp.y = ora.getDistanceTillNextBuoy();
            temp.z = i;
            _opponentCurrentBuoys.Add(temp);
        }

        _opponentCurrentBuoys.Add(new Vector3(_buoyIndicator, _playerBoat.getDistanceTillNextBuoy(), -1));

        _opponentCurrentBuoys = _opponentCurrentBuoys.OrderByDescending(x => x.x).ThenBy(x => x.y).ToList();

        int place = _opponentCurrentBuoys.IndexOf(_opponentCurrentBuoys.Where(x => x.z == -1).First()) +1;
        return place;
    }

    public void AwardPoints()
    {
        if(_finishOrder.Count != _opponentCurrentBuoys.Count)
        {
            for(int i = _finishOrder.Count; i < _opponentCurrentBuoys.Count; i++)
            {
                _finishOrder.Add(_opponenents[(int)_opponentCurrentBuoys[i].z]);
            }
        }

        for(int i = 0; i < _finishOrder.Count; i++)
        {
            float points = i +1;
            if (i == 0)
                points = 0.9f;
            if(_finishOrder[i].GetType() == typeof(OpponentRaceAi))
            {
                OpponentRaceAi ora = (OpponentRaceAi)_finishOrder[i];
                BoatRanking br = MainGameController.instance.databaseController.connection.Table<BoatRanking>().Where(x => x.PersonId == ora.person.id && x.BoatId == ora.person.BoatId).FirstOrDefault();
                if(br == null)
                {
                    br = new BoatRanking();
                    br.id = MainGameController.instance.databaseController.connection.Table<BoatRanking>().Count() + 1;
                    br.BoatId = ora.person.BoatId;
                    br.PersonId = ora.person.id;
                    br.New();
                }
                br.RacesSailed++;
                ora.person.RacesSailed++;
                if (points == 0.9f)
                {
                    br.RacesWon++;
                    ora.person.RacesWon++;
                }
                br.Points += points;
                ora.person.LifetimePoints += points;
                br.Save();
                ora.person.Save();
            }
            else
            {
                MainGameController.instance.achievementManager.AddAchievementProperty(AchievementProperties.Races, 1);
                Player player = MainGameController.instance.player;
                Person person = MainGameController.instance.databaseController.connection.Table<Person>().Where(x => x.Name.Equals(player.Name)).First();
                Boat b = player.GetActiveBoat();
                BoatRanking br = MainGameController.instance.databaseController.connection.Table<BoatRanking>().Where(x => x.PersonId == player.id && x.BoatId == b.id).FirstOrDefault();
                if(br == null)
                {
                    br = new BoatRanking();
                    br.id = MainGameController.instance.databaseController.connection.Table<BoatRanking>().Count() + 1;
                    Debug.Log(br.id);
                    br.PersonId = player.id;
                    br.BoatId = player.GetActiveBoat().id;
                    br.New();
                }
                br.RacesSailed++;

                person.RacesSailed++;
                if (points == 0.9f)
                {
                    MainGameController.instance.achievementManager.AddAchievementProperty(AchievementProperties.RacesWon, 1);
                    RequestManager.UpdateRequest(MainGameController.instance.player.GetActiveBoat(), _currentRace);
                    person.RacesWon++;
                    br.RacesWon++;
                }
                player.AddExperience(ExperienceController.ExperiencePerRacePlace * ((_opponenents.Count - i + 2)));
                br.Points += points;
                person.LifetimePoints += points;
                br.Save();
                person.Save();
            }

        }

        _pointsAwarded = true;
    }


    public void ContinueButtonClick()
    {
        if (!_pointsAwarded)
            AwardPoints();
        MainGameController.instance.canGoBack = true;
        MainGameController.instance.sceneController.LoadScene("VillageScene", false);
    }
}
