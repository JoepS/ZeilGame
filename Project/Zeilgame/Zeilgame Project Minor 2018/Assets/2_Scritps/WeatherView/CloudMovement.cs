using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CloudMovement : MonoBehaviour {

    [SerializeField] GameObject _cloudSetRain;
    [SerializeField] GameObject _cloudSetSnow;

    List<GameObject> _cloudsPool;

    [SerializeField] bool _spawnClouds;
    public bool SpawnClouds { set { _spawnClouds = value; } }
    [SerializeField] float _frequency;
    public float Frequency { set { _frequency = value; } }

    bool _snow;

    int _rainAmount;

    [SerializeField] Blink _thunderBlink;

	// Use this for initialization
	void Start () {
        _cloudsPool = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void SpawningClouds(bool value, float frequency, int rainAmount, bool snow)
    {
        _snow = snow;
        _frequency = frequency;
        _spawnClouds = value;
        _rainAmount = rainAmount;
        if (value)
            StartCoroutine(SpawnCloud());
        else
            StopCoroutine(SpawnCloud());
    }

    IEnumerator SpawnCloud()
    {
        while (_spawnClouds)
        {
            GameObject cloud = GetAvaliableCloud();
            cloud.GetComponent<Cloud>().Moving(true, _rainAmount, _thunderBlink);
            yield return new WaitForSeconds(100 / (_frequency + 1));
        }
    }

    GameObject GetAvaliableCloud()
    {
        string type = "Rain";
        if (_snow)
            type = "Snow";
        GameObject cloud = _cloudsPool.Where(x => !x.activeSelf && x.name.Contains(type)).FirstOrDefault();
        if(cloud == null)
        {
            if (!_snow)
            {
                cloud = GameObject.Instantiate(_cloudSetRain, this.transform);
                cloud.GetComponent<Cloud>().Speed = 100f;
            }
            else
            {
                cloud = GameObject.Instantiate(_cloudSetSnow, this.transform);
                cloud.GetComponent<Cloud>().Speed = 50f;
            }
            cloud.transform.localScale = Vector3.one;
            cloud.transform.localPosition = new Vector3((Screen.width/2) + cloud.GetComponent<RectTransform>().rect.width/2, 0, -1);
            cloud.SetActive(true);
            _cloudsPool.Add(cloud);
        }
        return cloud;
    }    
}
