using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeatherSceneController : MonoBehaviour {
    
    [SerializeField] GameObject _windImagesParent;
    [SerializeField] GameObject _windImagePrefab;

    [SerializeField] GameObject _worldMap;
    [SerializeField] GameObject _scrollviewContent;
    [SerializeField] GameObject _loadingImage;

    [SerializeField] Button _zoominButton;
    [SerializeField] Button _zoomOutButton;

    int _currZoom = 1;
    const int _maxZoom = 8;
    const int _minZoom = 1;

    int _loadedIndex;

	// Use this for initialization
	void Start () {
        StartCoroutine(getWindImages());
    }
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
            ZoomMap(true);
        if (Input.GetKeyDown(KeyCode.E))
            ZoomMap(false);
#endif
    }

    public IEnumerator getWindImages()
    {
        float worldMapSize = _windImagesParent.GetComponent<RectTransform>().sizeDelta.x;
        Debug.Log(worldMapSize);
        int amount = Mathf.RoundToInt(Mathf.Sqrt((worldMapSize / 2048)));
        Debug.Log("Getwindimageszoom: " + amount);

        int maxxy = (int)Mathf.Pow(2, amount);

        Vector2 playerpos = MainGameController.instance.player.getCurrentLocationLatLon();
        Vector3 pos = WorldMapController.LatitudeLongitudeToTileId(playerpos.x, playerpos.y, amount);

        int minx = (int)((pos.x - 1 >= 0) ? pos.x - 1 : 0);
        int maxx = (int)((pos.x + 1 < maxxy) ? pos.x + 1 : maxxy-1);
        int miny = (int)((pos.y - 1 >= 0) ? pos.y - 1 : 0);
        int maxy = (int)((pos.y + 1 < maxxy) ? pos.y + 1 : maxxy - 1);

        int lastIndex = 0;

        _windImagesParent.SetActive(false);
        _loadingImage.SetActive(true);
        _zoominButton.interactable = false;
        _zoomOutButton.interactable = false;

        _loadedIndex = MainGameController.instance.networkController.GetCurrentWeatherImageIndex();

        for (int fx = minx; fx <= maxx; fx++)
        {
            for (int fy = maxy; fy >= miny; fy--)
            {
                int index = MainGameController.instance.networkController.getWindImage(amount, fx, fy);
                Debug.LogError("Index = " + index);
                lastIndex = index;
                StartCoroutine(WaitForImageResponse(index, new Vector2(fx, fy)));
                yield return new WaitForSeconds(0.1f);
            }
        }

        StartCoroutine(viewWeatherMapWhenDone(lastIndex));
        //float parentpos = ((worldMapSize / 2048) - 1) * 1024;
        //_windImagesParent.transform.localPosition = new Vector3(parentpos, -parentpos);

        //MainGameController.instance.networkController.getWindImage(0, 0, 0);
        //StartCoroutine(WaitForImageResponse());
        
    }

    public IEnumerator viewWeatherMapWhenDone(int lastIndex)
    {
        while(lastIndex != _loadedIndex)
        {
            Debug.Log("" + lastIndex + " != " + _loadedIndex);
            yield return new WaitForSeconds(1);
        }
        _windImagesParent.SetActive(true);
        _loadingImage.SetActive(false);
        _zoominButton.interactable = true;
        _zoomOutButton.interactable = true;
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
        
        Debug.Log("CurZoom: " + _currZoom);
        float newsize = 4096 * _currZoom;
        Vector2 size = new Vector2(newsize, newsize);
        _scrollviewContent.GetComponent<RectTransform>().sizeDelta = size;
        _worldMap.GetComponent<RectTransform>().sizeDelta = size;
        _windImagesParent.GetComponent<RectTransform>().sizeDelta = size;

        for (int i = 0; i < _windImagesParent.transform.childCount; i++)
        {
            Destroy(_windImagesParent.transform.GetChild(i).gameObject);
        }

        StartCoroutine(getWindImages());
        _worldMap.GetComponent<WorldMapController>().ScrollToPosition(MainGameController.instance.player.getCurrentLocationLatLon());
        _worldMap.GetComponent<WorldMapController>().CreateLocationPointers(false);
    }

    IEnumerator WaitForImageResponse(int index, Vector2 pos)
    {
        Sprite weatherImage = (Sprite)MainGameController.instance.networkController.getResponse(Responses.WeatherImg, index);
        while(weatherImage == null)
        {
            Debug.Log("Waiting for response...");
            yield return new WaitForSeconds(1);
            weatherImage = (Sprite)MainGameController.instance.networkController.getResponse(Responses.WeatherImg, index);
        }
        Debug.Log("Got weather image " + index + " / " + weatherImage);
        GameObject windImage = GameObject.Instantiate(_windImagePrefab);
        windImage.transform.SetParent(_windImagesParent.transform);
        windImage.name = "WindImage" + pos;
        windImage.GetComponent<Image>().sprite = weatherImage;
        windImage.GetComponent<Image>().color = Color.white;
        windImage.GetComponent<Image>().mainTexture.filterMode = FilterMode.Point;
        windImage.GetComponent<RectTransform>().sizeDelta = new Vector2(2048, 2048);
        windImage.transform.localScale = Vector3.one;
        windImage.transform.localPosition = new Vector2(pos.x * 2048, pos.y * -2048);
        _loadedIndex += 1;
        Debug.LogError("Loaded index + 1 = " + _loadedIndex);
    }


    public void WeatherInRect() {
        Vector2 playerLoc = MainGameController.instance.player.getCurrentLocationLatLon();
        float latbot = 0;
        if (playerLoc.x - 10 > -180)
            latbot = playerLoc.x - 10;
        float lattop = 0;
        if (playerLoc.x + 10 < 180)
            lattop = playerLoc.x + 10;
        float lonleft = 0;
        if (playerLoc.y - 10 > -85)
            lonleft = playerLoc.y - 10;
        float lonright = 0;
        if (playerLoc.y + 10 < 85)
            lonright = playerLoc.y + 10;

        MainGameController.instance.networkController.getWeatherInRectangle(lonleft, latbot, lonright, lattop, 1);
        StartCoroutine(WaitForResponse());
    }

    IEnumerator WaitForResponse()
    {
        RectangularWeatherData data = (RectangularWeatherData)MainGameController.instance.networkController.getResponse(Responses.WeatherRect, -1);
        while(data == null)
        {
            Debug.Log("Waiting for response...");
            yield return new WaitForSeconds(1);
            data = (RectangularWeatherData)MainGameController.instance.networkController.getResponse(Responses.WeatherRect, -1);
        }
        Debug.Log("Got a response:");
        Debug.Log(data);
    }

    public void OnBackButtonClick()
    {
        MainGameController.instance.sceneController.OneSceneBack();
    }
}
