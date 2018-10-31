using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RepairMiniGameController : MonoBehaviour {

    const string TutorialClosedPlayerPref = "RepairMiniGameTutorialSeen";

    float _damage = 0;
    float _maxDamage = 100;
    float perClick = 0.1f;

    float _timer = 10;
    bool _timerStart = false;
    bool _canClick = true;

    [SerializeField] Slider _slider;

    [SerializeField] Text _timerText;
    [SerializeField] Text _percentageText;

    [Range(1, 10)] int _difficulty;

    Boat _boatToRepair;

    [SerializeField] GameObject _tutorialPanel;

    [SerializeField] Text _tapToStartText;

	// Use this for initialization
	void Start () {
       if(PlayerPrefs.GetInt(TutorialClosedPlayerPref) > 0)
        {
            //OnTutorialClick();
        }

    }

    public void SetBoatToRepair(Boat boatToRepair)
    {
        _boatToRepair = boatToRepair;
        _maxDamage = _boatToRepair.GetMaxDamage();
        _damage = _maxDamage - _boatToRepair.Damage;

        float diffPercent = (_boatToRepair.Damage) / (_maxDamage);
        SetDifficulty(diffPercent);
        perClick = (_maxDamage - _damage) / (_timer * _difficulty);

        _slider.minValue = 0;
        _slider.maxValue = _maxDamage;
        _slider.value = _damage;

        _timerText.text = "" + _timer;
        _percentageText.text = ((_damage / _maxDamage) * 100) + "%";
    }

    // Update is called once per frame
    void Update () {
        if (_timerStart) {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                _timer = 0;
                Done();
            }
            _timerText.text = "" + Mathf.Round(_timer * 100) / 100;
        }
	}

    public void SetDifficulty(float percent)
    {
        if(percent < 0.1)
        {
            _difficulty = 2;
        }
        else if(percent < 0.25)
        {
            _difficulty = 3;
        }
        else if (percent < 0.6)
        {
            _difficulty = 4;
        }
        else if (percent < 0.9)
        {
            _difficulty = 6;
        }
        else
        {
            _difficulty = 8;
        }
    }

    public void OnPanelClick()
    {
        if (_canClick)
        {
            if (Mathf.Approximately(_damage, _boatToRepair.GetMaxDamage() - _boatToRepair.Damage))
            {
                _timerStart = true;
                _tapToStartText.gameObject.SetActive(false);
            }
            _damage+= perClick;
            _slider.value = _damage;
            float percentage = ((_damage / _maxDamage) * 100);
            percentage = Mathf.Round(percentage * 100) / 100;
            _percentageText.text = percentage + "%";
            if (_damage >= _maxDamage)
            {
                _damage = _maxDamage;

                _slider.value = _damage;
                percentage = ((_damage / _maxDamage) * 100);
                percentage = Mathf.Round(percentage * 100) / 100;
                _percentageText.text = percentage + "%";
                Done();
            }
        }
    }

    public void Done()
    {
        _timerStart = false;
        _canClick = false;
        if(_slider.value == _maxDamage)
        {
            Debug.LogError("You Completed the Minigame!");
            _boatToRepair.Damage = 0;
            _boatToRepair.Save();
        }
        else
        {
            _boatToRepair.Damage *= 1.1f;
            if (_boatToRepair.Damage > _boatToRepair.GetMaxDamage())
                _boatToRepair.Damage = _boatToRepair.GetMaxDamage();
            Debug.LogError("You Failed the Minigame, your boat has " + _boatToRepair.Damage + " damage"); 
            _boatToRepair.Save();
        }
    }

    public void OnTutorialClick()
    {
        PlayerPrefs.SetInt(TutorialClosedPlayerPref, 1);
        _tapToStartText.gameObject.SetActive(true);
        _tutorialPanel.SetActive(false);
    }
}
