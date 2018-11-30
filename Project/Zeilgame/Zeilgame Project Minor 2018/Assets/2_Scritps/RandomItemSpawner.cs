using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemSpawner : MonoBehaviour{

    [SerializeField] SeaMovement _seaMovement;
    [SerializeField] GameObject _randomItemCrate;
    [SerializeField] GameObject _itemPopupPrefab;

    bool _coroutineStarted = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnRandomItem();
        }
        if (_seaMovement.GetSeaBlocks() != null && !_coroutineStarted)
        {
            _coroutineStarted = true;   
            StartCoroutine(WaitToSpawnRandomItem());
        }
	}

    IEnumerator WaitToSpawnRandomItem()
    {
        while (true)
        {
            float waitTime = Random.Range(60, 300);
            yield return new WaitForSeconds(waitTime);
            //SpawnRandomItem
            SpawnRandomItem();
        }
    }

    public void SpawnRandomItem()
    {
        GameObject go = GameObject.Instantiate(_randomItemCrate);
        go.transform.SetParent(this.transform);
        go.transform.localScale = Vector3.one;
        List<GameObject> seaBlocks = _seaMovement.GetSeaBlocks();
        go.GetComponent<RandomItemCrate>().SeaBlocks = seaBlocks;
        int random = Random.Range(0, 100);
        if (random % 2 == 0)
            go.GetComponent<RandomItemCrate>().RandomGoldAmount = Random.Range(10, 100);
        else {
            int randomindex = Random.Range(1, MainGameController.instance.databaseController.connection.Table<Item>().Count() + 1);
            go.GetComponent<RandomItemCrate>().RandomItem = MainGameController.instance.databaseController.connection.Table<Item>().Where(x => x.id == randomindex).First();

        }


        go.GetComponent<RandomItemCrate>().ItemPopupPrefab = _itemPopupPrefab;
    }  
}
