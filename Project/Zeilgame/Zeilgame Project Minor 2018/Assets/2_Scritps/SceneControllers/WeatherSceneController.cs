using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    static List<OWMWeatherImage> _downloadedWeatherImages;

	// Use this for initialization
	void Start () {
        if(_downloadedWeatherImages == null)
        {
            _downloadedWeatherImages = new List<OWMWeatherImage>();
        }
        StartCoroutine(getWindImages());
        _worldMap.GetComponent<WorldMapController>().SetCanMove(false);
        Route r = MainGameController.instance.player.GetCurrentRoute();
        if(r!= null)
        {
            _worldMap.GetComponent<WorldMapController>().ShowRoute(r);
        }
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
        float worldMapSize = 4096 * _currZoom;//_windImagesParent.GetComponent<RectTransform>().sizeDelta.x;
        int amount = Mathf.RoundToInt(Mathf.Sqrt((worldMapSize / 2048)));
        int maxxy = (int)Mathf.Pow(2, amount);

        Vector2 playerpos = MainGameController.instance.player.getCurrentLocationLatLon();
        Vector3 pos = WorldMapController.LatitudeLongitudeToTileId(playerpos.x, playerpos.y, amount);

        int minx = (int)((pos.x - 2 >= 0) ? pos.x - 2 : 0);
        int maxx = (int)((pos.x + 2 < maxxy) ? pos.x + 2 : maxxy-1);
        int miny = (int)((pos.y - 2 >= 0) ? pos.y - 2 : 0);
        int maxy = (int)((pos.y + 2 < maxxy) ? pos.y + 2 : maxxy - 1);

        int lastIndex = 0;

        _windImagesParent.SetActive(false);
        _loadingImage.SetActive(true);
        _zoominButton.interactable = false;
        _zoomOutButton.interactable = false;

        //_loadedIndex = MainGameController.instance.networkController.GetCurrentWeatherImageIndex();
        int startIndex = _loadedIndex;
        int index = startIndex;
        for (int fx = minx; fx <= maxx; fx++)
        {
            for (int fy = maxy; fy >= miny; fy--)
            {
                OWMWeatherImage oWMWeatherImage = _downloadedWeatherImages.Where(x => x.ZoomLevel == _currZoom && x.Position == new Vector2(fx, fy) && (x.DownloadTime - System.DateTime.Now).Hours < 1).FirstOrDefault();
                
                if (oWMWeatherImage == null)
                {
                    index = MainGameController.instance.networkController.getWindImage(amount, fx, fy);
                }
                else
                {
                    index++; 
                    oWMWeatherImage.Index = index;
                }
                lastIndex = index;
                StartCoroutine(WaitForImageResponse(index, new Vector2(fx, fy)));
                yield return new WaitForSeconds(0.1f);
            }
        }
        StartCoroutine(viewWeatherMapWhenDone(startIndex, lastIndex));
        //float parentpos = ((worldMapSize / 2048) - 1) * 1024;
        //_windImagesParent.transform.localPosition = new Vector3(parentpos, -parentpos);

        //MainGameController.instance.networkController.getWindImage(0, 0, 0);
        //StartCoroutine(WaitForImageResponse());

    }

    public IEnumerator viewWeatherMapWhenDone(int startIndex, int lastIndex)
    {
        while (lastIndex != _loadedIndex)
        {
            yield return new WaitForSeconds(1);
        }
        for (int i = 0; i < _windImagesParent.transform.childCount; i++)
        {
            int trying = 0;
            int.TryParse((_windImagesParent.transform.GetChild(i).name.Split('-')[0]), out trying);
            if (trying <= startIndex)
            {
                Destroy(_windImagesParent.transform.GetChild(i).gameObject);
            }
            else
            {
                _windImagesParent.transform.GetChild(i).gameObject.SetActive(true);
            }
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

        float newsize = 4096 * _currZoom;
        Vector2 size = new Vector2(newsize, newsize);
        _worldMap.GetComponent<WorldMapController>().SetZoom(_currZoom);
        _worldMap.GetComponent<WorldMapController>().SetNewSize(size);
        //_windImagesParent.GetComponent<RectTransform>().sizeDelta = size;
        for (int i = 0; i < _windImagesParent.transform.childCount; i++)
        {
            //Destroy(_windImagesParent.transform.GetChild(i).gameObject);
            GameObject child = _windImagesParent.transform.GetChild(i).gameObject;
            child.transform.localScale = increase ? new Vector3(2, 2, 2) : new Vector3(0.5f, 0.5f, 0.5f); 
            Vector2 pos = child.transform.localPosition;
            pos.x /= 2048;
            pos.y /= -2048;

            float zoom = _currZoom;
            if (zoom > 2 && increase)
                zoom = 2;
            else if (!increase)
            {
                zoom = 0.5f;
            }
            child.transform.localPosition = new Vector2(2048 * zoom * pos.x, -(2048 * zoom) * pos.y);
            //if (increase)
            //{
            //    child.transform.localPosition += new Vector3(1024, -1024, 0);
            //}
            //else
            //{
            //    child.transform.localPosition -= new Vector3(512, -512, 0);
            //}
        }
        StartCoroutine(getWindImages());
        _worldMap.GetComponent<WorldMapController>().ScrollToPosition(MainGameController.instance.player.getCurrentLocationLatLon());
        _worldMap.GetComponent<WorldMapController>().CreateLocationPointers(false);
    }

    IEnumerator WaitForImageResponse(int index, Vector2 pos)
    {
        OWMWeatherImage weatherImage = _downloadedWeatherImages.Where(x => x.ZoomLevel == _currZoom && x.Position == pos && (x.DownloadTime - System.DateTime.Now).Hours < 1).FirstOrDefault();
            //(OWMWeatherImage)MainGameController.instance.networkController.getResponse(Responses.WeatherImg, index);
        while(weatherImage == null)
        {
            yield return new WaitForSeconds(1);
            weatherImage = (OWMWeatherImage)MainGameController.instance.networkController.getResponse(Responses.WeatherImg, index);
            if(weatherImage != null)
            {
                weatherImage.Position = pos;
                weatherImage.ZoomLevel = _currZoom;
                _downloadedWeatherImages.Add(weatherImage);
            }
        }
        index = weatherImage.Index;

        GameObject windImage = GameObject.Instantiate(_windImagePrefab);
        windImage.transform.SetParent(_windImagesParent.transform);
        windImage.name = index + "-WindImage" + pos;
        windImage.GetComponent<Image>().sprite = weatherImage.Image;
        windImage.GetComponent<Image>().color = Color.white;
        windImage.GetComponent<Image>().mainTexture.filterMode = FilterMode.Point;
        windImage.GetComponent<RectTransform>().sizeDelta = new Vector2(2048, 2048);
        windImage.transform.localScale = Vector3.one;
        windImage.transform.localPosition = new Vector2(pos.x * 2048, pos.y * -2048) - new Vector2(1024, -1024);
        windImage.SetActive(false);
        
        _loadedIndex += 1;
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
            yield return new WaitForSeconds(1);
            data = (RectangularWeatherData)MainGameController.instance.networkController.getResponse(Responses.WeatherRect, -1);
        }
    }

    public void OnBackButtonClick()
    {
        MainGameController.instance.sceneController.OneSceneBack();
    }
}
