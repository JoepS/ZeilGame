using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeatherViewer : MonoBehaviour {

    [SerializeField] GameObject _dropPrefab;

    List<GameObject> _rainDropPool;

    public int _amountOfDrops = 50;

    System.Random random;
    // Use this for initialization
    void Start () {
        random = new System.Random((int)System.DateTime.Now.ToFileTimeUtc());
        _rainDropPool = new List<GameObject>();
        StartCoroutine(Rain());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator Rain()
    {
        while(true)
        {
            SimulateRainTick();
            yield return new WaitForSeconds(0.1f);
        }
    }



    void SimulateRainTick()
    {
        float width = this.GetComponent<RectTransform>().rect.width;
        float distance = width / (float)(_amountOfDrops);
        for (int i = 0; i < _amountOfDrops; i++)
        {
            GameObject drop = GetRainDrop();
            drop.transform.localPosition = new Vector3(random.Next(0, (int)width), random.Next(-100, 100), 0);
        }
    }

    GameObject GetRainDrop()
    {
        GameObject drop = _rainDropPool.Where(x => !x.activeSelf).FirstOrDefault();
        if(drop == null)
        {
            drop = GameObject.Instantiate(_dropPrefab, this.transform);
            drop.transform.localScale = Vector3.one;
            _rainDropPool.Add(drop);
        }
        drop.SetActive(true);
        return drop;
    }
}
