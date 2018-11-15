using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WavesController : MonoBehaviour {

    [SerializeField] GameObject _wavePrefab;

    List<GameObject> _waves;

    int _activeWaveCount;
    [SerializeField] int _maxWaves = 10;
    [SerializeField] float _waveDistance = 1000;

    [SerializeField] BoatMovement _playerBoat;

	// Use this for initialization
	void Start () {
        _waves = new List<GameObject>();
        StartCoroutine(GenerateWaves());
    }

    IEnumerator GenerateWaves()
    {
        Vector3 windDir = Vector3.zero;
        float windSpeed = 10;
        if(PickRaceSceneController.CurrentWeatherData != null)
        {
            windDir.z = (float)RaceSceneController.instance.windAngle;
            windSpeed = (float)RaceSceneController.instance.windSpeed * 10;
        }
        while (true)
        {
            while (_activeWaveCount < _maxWaves)
            {
                GameObject go = GetWave();
                go.SetActive(true);
                go.transform.position = GetPositionOutOfScreen();
                go.GetComponent<Wave>().SetMovement(windDir, windSpeed, Random.Range(0f, 1f));
                _activeWaveCount++;
                yield return new WaitForEndOfFrame();
            }
            foreach (GameObject go in _waves)
            {
                float distance = Vector2.Distance(go.transform.position, _playerBoat.transform.position);
                if (distance > (Screen.height / 2) + _wavePrefab.GetComponent<RectTransform>().rect.width && Wave.CanMove)
                {
                    go.SetActive(false);
                    _activeWaveCount--;
                }
                else
                {
                    go.GetComponent<Wave>().SetMovement(windDir, windSpeed, go.transform.localScale.y);
                }
                yield return new WaitForEndOfFrame();
            }
            if (PickRaceSceneController.CurrentWeatherData != null)
            {
                windDir.z = (float)RaceSceneController.instance.windAngle;
                windSpeed = (float)RaceSceneController.instance.windSpeed * 10;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    Vector3 GetPositionOutOfScreen()
    {
        Vector3 pos = _playerBoat.transform.position;
        float rand = Random.Range(1, 5);
        float waveWidth = _wavePrefab.GetComponent<RectTransform>().rect.width;
        float waveHeight = _wavePrefab.GetComponent<RectTransform>().rect.height;
        if (rand == 1)
        {
            //Top of screen
            pos += new Vector3(Random.Range((-Screen.width/2) -waveWidth, (Screen.width/2) + waveWidth), (Screen.height / 2) + Random.Range(waveHeight/2, waveHeight * 2));
        }
        else if(rand == 2)
        {
            //Right of screen
            pos += new Vector3((Screen.width /2) + Random.Range(waveWidth / 2, waveWidth * 2), Random.Range(-(Screen.height / 2) - waveHeight, (Screen.height/2) + waveHeight));
        }
        else if (rand == 3)
        {
            //Bottom of screen
            pos += new Vector3(Random.Range((-Screen.width / 2) - waveWidth, (Screen.width / 2) + waveWidth), -(Screen.height / 2) - Random.Range(waveHeight / 2, waveHeight * 2));
        }
        else
        {
            //Let of screen
            pos += new Vector3(-(Screen.width / 2) - Random.Range(waveWidth / 2, waveWidth * 2), Random.Range(-(Screen.height / 2) - waveHeight, (Screen.height / 2) + waveHeight));
        }

        return pos;
    }

    GameObject GetWave()
    {
        GameObject wave = _waves.Where(x => !x.activeSelf).FirstOrDefault();
        if(wave == null)
        {
            wave = SpawnWave();
        }
        return wave;
    }

    GameObject SpawnWave()
    {
        GameObject wave = GameObject.Instantiate(_wavePrefab);
        wave.transform.SetParent(this.transform);
        wave.transform.localScale = Vector3.one;
        wave.gameObject.SetActive(false);
        _waves.Add(wave);
        return wave;
    }
}
