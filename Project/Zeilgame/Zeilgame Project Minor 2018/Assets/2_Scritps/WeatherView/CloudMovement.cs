using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CloudMovement : MonoBehaviour {

    [SerializeField] GameObject _cloudSet;

    List<GameObject> _cloudsPool;

    [SerializeField] bool _spawnClouds;
    public bool SpawnClouds { set { _spawnClouds = value; } }
    [SerializeField] float _frequency;
    public float Frequency { set { _frequency = value; } }

	// Use this for initialization
	void Start () {
        _cloudsPool = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void SpawningClouds(bool value, float frequency)
    {
        _frequency = frequency;
        _spawnClouds = value;
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
            cloud.GetComponent<Cloud>().Moving(true);
            yield return new WaitForSeconds(100 / (_frequency + 1));
        }
    }

    GameObject GetAvaliableCloud()
    {
        GameObject cloud = _cloudsPool.Where(x => !x.activeSelf).FirstOrDefault();
        if(cloud == null)
        {
            cloud = GameObject.Instantiate(_cloudSet, this.transform);
            cloud.transform.localScale = Vector3.one;
            cloud.transform.localPosition = new Vector3(-(Screen.width/2) - cloud.GetComponent<RectTransform>().rect.width/2, 0, 0);
            cloud.SetActive(true);
            _cloudsPool.Add(cloud);
        }
        return cloud;
    }    
}
