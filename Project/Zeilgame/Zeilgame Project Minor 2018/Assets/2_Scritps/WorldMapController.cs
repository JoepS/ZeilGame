﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class WorldMapController : MonoBehaviour {

    [SerializeField] GameObject _routePointer;
    [SerializeField] GameObject _routePointerParent;
    [SerializeField] GameObject _locationPointer;
    [SerializeField] GameObject _loctionPointersParent;
    [SerializeField] ScrollRect _mapScrollView;
    [SerializeField] GameObject _PlayerPointer;

    List<MapLocationPointer> _locationPointers;

    Route _currentRouteShowing;

    float _MapSize = 4096;

    public  const int EarthRadius = 6378137; //no seams with globe example
    private const float OriginShift = 2f * Mathf.PI * EarthRadius * 2f;

    private void Awake()
    {
        if (MainGameController.instance != null)
        {
            _locationPointers = new List<MapLocationPointer>();
            CreateLocationPointers(false);
        }
    }

    // Use this for initialization
    void Start () {

        if (MainGameController.instance.player != null)
        {
            Vector2 playerpos = new Vector2(MainGameController.instance.player.CurrentLocationLat
                                            , MainGameController.instance.player.CurrentLocationLon);
            ScrollToPosition(playerpos);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreateLocationPointers(bool viewOnlyAvaliableLocations)
    {
        _MapSize = this.GetComponent<RectTransform>().sizeDelta.x;
        _locationPointers.Clear();
        for (int i = 0; i < _loctionPointersParent.transform.childCount; i++)
        {
            Destroy(_loctionPointersParent.transform.GetChild(i).gameObject);
        }
        if (MainGameController.instance != null)
        {
            List<Location> locations = MainGameController.instance.databaseController.connection.Table<Location>().ToList();
            foreach (Location l in locations)
            {
                if (MainGameController.instance.player != null) 
                    if (MainGameController.instance.player.GetCurrentLocation().Name != l.Name)
                            CreateLocationPointer(l);
            }
        }
        if (MainGameController.instance.player != null)
            _PlayerPointer.transform.localPosition = WorldToMap(MainGameController.instance.player.getCurrentLocationLatLon());
        if(_currentRouteShowing != null)
        {
            ShowRoute(_currentRouteShowing);
        }
    }

    void CreateLocationPointer(Location l)
    {
        GameObject locationPointer = GameObject.Instantiate(_locationPointer);
        locationPointer.transform.SetParent(_loctionPointersParent.transform);
        locationPointer.transform.localScale = Vector3.one;
        locationPointer.transform.localPosition = WorldToMap(new Vector2(l.lat, l.lon));
        locationPointer.GetComponent<MapLocationPointer>().SetData(l);
        _locationPointers.Add(locationPointer.GetComponent<MapLocationPointer>());
    }

    public void ShowRoute(Route r)
    {
        /*string s = "";
        foreach(Vector2 v2 in r.GetListRoute())
        {
            s += JsonUtility.ToJson(MapToWorld(v2)) + ",";
        }
        Debug.Log(s);*/
        for (int i = 0; i < _routePointerParent.transform.childCount; i++)
        {
            Destroy(_routePointerParent.transform.GetChild(i).gameObject);
        }
        foreach (Vector2 c2 in r.GetListRoute())
        {
            GameObject routePointer = GameObject.Instantiate(_routePointer);
            routePointer.transform.SetParent(_routePointerParent.transform);
            routePointer.transform.localPosition = WorldToMap(c2);            
        }
        _currentRouteShowing = r;
    }

    public List<MapLocationPointer> GetLocationPointers()
    {
        return _locationPointers;
    }

    public void ScrollToPosition(Vector2 position)
    {

        Vector2 mapPos = WorldToMap(position);
        float xval = MainGameController.Map(mapPos.x, -_MapSize/2, _MapSize/2, 0, 1);
        float yval = MainGameController.Map(mapPos.y, -_MapSize/2, _MapSize/2, 0, 1);

        //float yval = MainGameController.Map(position.x, -90, 90, 0, 1);
        //float xval = MainGameController.Map(position.y, -180, 180, 0, 1);
        _mapScrollView.horizontalNormalizedPosition = xval;
        _mapScrollView.verticalNormalizedPosition = yval;
    }

    Vector2 MapToWorld(Vector2 mapCoordinates)
    {
        Vector2 worldCoordinates = new Vector2();

        float coy = mapCoordinates.y;
        float cox = mapCoordinates.x;
        coy *= -1;
        float longitude = (cox - 0f) /
            (_MapSize / 360f);
        float latitude = (2f * Mathf.Atan(Mathf.Exp(
            (coy - 0f) / -(_MapSize / (2f * Mathf.PI))))
            - Mathf.PI / 2f) * (180f / Mathf.PI);

        worldCoordinates.x = latitude;
        worldCoordinates.y = longitude;

        return worldCoordinates;
    }

    Vector2 WorldToMap(Vector2 worldCoordinates)
    {
        Vector2 mapCoordinates = new Vector2();
        var x = Mathf.Round(0f + (worldCoordinates.y * (_MapSize / 360)));
        var f = Mathf.Min(
            Mathf.Max(
                 Mathf.Sin(worldCoordinates.x * (Mathf.PI / 180f)),
                -0.9999f),
            0.9999f);
        var y = Mathf.Round(0f + .5f *
            Mathf.Log((1f + f) / (1f - f)) * -(_MapSize / (2f * Mathf.PI)));
        y *= -1;

        mapCoordinates.x = x;
        mapCoordinates.y = y;

        return mapCoordinates;
    }

    public static float calculateDistanceLatLongKM(Vector2 from, Vector2 to)
    {
        from.x = from.x * Mathf.Deg2Rad;
        from.y = from.y * Mathf.Deg2Rad;

        to.x = to.x * Mathf.Deg2Rad;
        to.y = to.y * Mathf.Deg2Rad;

        float dlat = to.x - from.x;
        float dlon = to.y - from.y;

        float a = Mathf.Pow((Mathf.Sin(dlat / 2)), 2) + Mathf.Cos(from.x) * Mathf.Cos(to.x) * Mathf.Pow((Mathf.Sin(dlon / 2)), 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1f - a));
        float d = 6373 * c;

        return d;
    }

    public static float calculateRouteDistanceKM(Route r)
    {
        float dist = 0;

        for(int i = 1; i < r.GetListRoute().Count; i++)
        {
            Vector2 a = r.GetListRoute()[i - 1];
            Vector2 b = r.GetListRoute()[i];
            dist += calculateDistanceLatLongKM(a, b);
        }

        return dist;
    }

    /// Source: https://github.com/mapbox/mapbox-unity-sdk/blob/develop/sdkproject/Assets/Mapbox/Unity/Utilities/Conversions.cs#L204
    /// <summary>
    /// Gets the xy tile ID at the requested zoom that contains the WGS84 lat/lon point.
    /// See: http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames.
    /// </summary>
    /// <param name="latitude"> The latitude. </param>
    /// <param name="longitude"> The longitude. </param>
    /// <param name="zoom"> Zoom level. </param>
    /// <returns> A <see cref="T:Mapbox.Map.UnwrappedTileId"/> xy tile ID. </returns>
    public static Vector3 LatitudeLongitudeToTileId(float latitude, float longitude, int zoom)
    {
        var x = (int)Mathf.Floor((longitude + 180.0f) / 360.0f * Mathf.Pow(2.0f, zoom));
        var y = (int)Mathf.Floor((1.0f - Mathf.Log(Mathf.Tan(latitude * Mathf.PI / 180.0f)
                + 1.0f / Mathf.Cos(latitude * Mathf.PI / 180f)) / Mathf.PI) / 2.0f * Mathf.Pow(2.0f, zoom));
        return new Vector3(x, y, zoom);
    }

}