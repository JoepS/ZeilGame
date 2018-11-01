﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameController : MonoBehaviour {

    const string StartMenuSceneName = "StartMenu";

    public static MainGameController instance;

    public SceneController sceneController { get { return _sceneController; } }

    [SerializeField] SceneController _sceneController;

    public DatabaseController databaseController { get { return _databaseController; } }

    DatabaseController _databaseController;

    public OpenWeatherMapController openWeatherMapController { get { return _openWeatherMapController; } }

    [SerializeField] OpenWeatherMapController _openWeatherMapController;

    public NetworkController networkController { get { return _networkController; } }

    [SerializeField] NetworkController _networkController;

    public LocalizationManager localizationManager { get { return _localizationManager; } }

    [SerializeField] LocalizationManager _localizationManager;

    public AchievementManager achievementManager { get { return _achievementManager; } }

    [SerializeField] AchievementManager _achievementManager;

    public PopupManager popupManager { get { return _popupManager; } }

    [SerializeField] PopupManager _popupManager;

    public Player player { get { return _player; } set { _player = value; } }

    Player _player;

    public bool canGoBack { get { return _canGoBack; } set { _canGoBack = value; } }

    bool _canGoBack = true;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != StartMenuSceneName && instance == null && SceneManager.GetActiveScene().name != "TestScene")
        {
            _sceneController.LoadScene(StartMenuSceneName, false);
        }
    }

    // Use this for initialization
    void Start () {

		if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            _databaseController = new DatabaseController("DB.db");
            achievementManager.Reset();
            if (_databaseController.connection.Table<Player>().Count() > 0)
            {

                _player = _databaseController.connection.Table<Player>().First();
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxis("Cancel") > 0)
        {
            if (!canGoBack)
                _sceneController.LoadScene(StartMenuSceneName, false);
            else if (!_sceneController.OneSceneBack())
                CloseGame();
        }
	}

    public void CloseGame()
    {
        Application.Quit();
    }

    public static float Map(float val, float srcMin, float srcMax, float dstMin, float dstMax)
    {
        //if (val >= srcMax) return dstMax;
        //if (val <= srcMin) return dstMin;
        return dstMin + (val - srcMin) / (srcMax - srcMin) * (dstMax - dstMin);
    }

    public static float MapClamped(float val, float srcMin, float srcMax, float dstMin, float dstMax)
    {
        if (val >= srcMax) return dstMax;
        if (val <= srcMin) return dstMin;
        return dstMin + (val - srcMin) / (srcMax - srcMin) * (dstMax - dstMin);
    }

    private void OnApplicationQuit()
    {
        if (_player != null)
        {
            _player.LastInGame = JsonUtility.ToJson((JsonDateTime)DateTime.Now);
            _player.Save();
        }
#if !UNITY_EDITOR
        _databaseController.EncryptDB();
#endif
        _databaseController = null;
    }
}

public struct JsonDateTime
{
    public long value;
    public static implicit operator DateTime(JsonDateTime jdt)
    {
        return DateTime.FromFileTimeUtc(jdt.value);
    }
    public static implicit operator JsonDateTime(DateTime dt)
    {
        JsonDateTime jdt = new JsonDateTime();
        jdt.value = dt.ToFileTimeUtc();
        return jdt;
    }
}

[Serializable]
public class ListInt
{
    public List<int> list;
}

[Serializable]
public class ListFloat
{
    public List<float> list;
}

[Serializable]
public class ListV2
{
    public List<Vector2> list;
}