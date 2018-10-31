using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System;

[Table("Route")]
public class Route : Model {
    public int from { get; set; }
    Location fromLocation { get; set; }
    public int to { get; set; }
    Location toLocation { get; set; }
    public string route { get; set; }
    public ListV2 listroute;

    bool _reversed = false;

    float _distanceLeft;

    public float GetDistanceLeft()
    {
        return _distanceLeft;
    }

    public void SetDistanceLeft(float distanceLeft)
    {
        _distanceLeft = distanceLeft;
    }

    public Location GetToLocation()
    {
        if(toLocation == null)
        {
            toLocation = MainGameController.instance.databaseController.connection.Table<Location>().Where(x => x.id == to).First();
        }

        return toLocation;
    }

    public Location GetFromLocation()
    {
        if(fromLocation == null)
        {
            fromLocation = MainGameController.instance.databaseController.connection.Table<Location>().Where(x => x.id == from).First();
        }
        return fromLocation;
    }

    public TimeSpan GetTimeLeft(float speed)
    {
        float time = _distanceLeft / speed;
        float timeminutes = time - (int)time;
        int minutes = (int)(timeminutes * 60);
        float timeseconds = timeminutes - (float)(minutes / 60f);
        int seconds = (int)(timeseconds * 3600);
        TimeSpan timeSpan = new TimeSpan((int)time, minutes, seconds);
        return timeSpan;
    }


    public List<Vector2> GetListRoute()
    {
        if (listroute == null)
        {
            listroute = JsonUtility.FromJson<ListV2>(route);
        }

        return listroute.list;
    }

    public void CheckIfNeedToBeReversed(Location startLocation)
    {
        if(from != startLocation.id && !_reversed) 
        {
            if (listroute == null)
                listroute = JsonUtility.FromJson<ListV2>(route);
            listroute.list.Reverse();
            _reversed = true ;
        }
    }

    public float GetRouteDistanceFromPosition(Vector2 position, Location startLocation)
    {

        CheckIfNeedToBeReversed(startLocation);

        float distance = 0;
        bool foundPlace = false;
        for(int i = 1; i< GetListRoute().Count; i++)
        {
            Vector2 prev = GetListRoute()[i - 1];
            Vector2 cur = GetListRoute()[i];

            if (foundPlace)
            {
                distance += WorldMapController.calculateDistanceLatLongKM(prev, cur);
            }
            else
            {
                if ((Mathf.Approximately(prev.x, position.x) && Mathf.Approximately(prev.y, position.y)) || position == prev || Vector2.Distance(position, prev) < 0.00001f)
                {
                    distance += WorldMapController.calculateDistanceLatLongKM(position, cur);
                    foundPlace = true;
                }
                else if (prev.x < cur.x && prev.y < cur.y)
                {
                    if (position.x > prev.x && position.x < cur.x &&
                       position.y > prev.y && position.y < cur.y)
                    {
                        distance += WorldMapController.calculateDistanceLatLongKM(position, cur);
                        foundPlace = true;
                    }
                }
                else if (prev.x > cur.x && prev.y < cur.y)
                {
                    if (position.x < prev.x && position.x > cur.x &&
                       position.y > prev.y && position.y < cur.y)
                    {
                        distance += WorldMapController.calculateDistanceLatLongKM(position, cur);
                        foundPlace = true;
                    }
                }
                else if (prev.x > cur.x && prev.y > cur.y)
                {
                    if (position.x < prev.x && position.x > cur.x &&
                       position.y < prev.y && position.y > cur.y)
                    {
                        distance += WorldMapController.calculateDistanceLatLongKM(position, cur);
                        foundPlace = true;
                    }
                }
                else if (prev.x < cur.x && prev.y > cur.y)
                {
                    if (position.x > prev.x && position.x < cur.x &&
                       position.y < prev.y && position.y > cur.y)
                    {
                        distance += WorldMapController.calculateDistanceLatLongKM(position, cur);
                        foundPlace = true;
                    }
                }
            }
        }

        return distance;
    }

    public Vector2 GetPositionInRoute(Location startLocation)
    {
        CheckIfNeedToBeReversed(startLocation);

        Vector2 position = Vector2.zero;

        float distance = 0;
        float distanceDone = WorldMapController.calculateRouteDistanceKM(this) - GetDistanceLeft();
        for(int i = 1; i < GetListRoute().Count; i++)
        {
            Vector2 prev = GetListRoute()[i - 1];
            Vector2 cur = GetListRoute()[i];
            float prevdistance = distance;
            distance += WorldMapController.calculateDistanceLatLongKM(prev, cur);

            if(distance > distanceDone)
            {
                float tempdist = distance - prevdistance;
                float tempdistDone = distanceDone - prevdistance;
                float tempPosition = tempdistDone / tempdist;
                position = Vector2.Lerp(prev, cur, tempPosition);
                break;
            }
        }


        return position;
    }

    public override string ToString()
    {
        string s = id + " / " + from + " / " + to;
        //s += " / " + GetListRoute();
        foreach(Vector2 v in GetListRoute())
        {
            s += " / " + v;
        }
        return s;
    }

    public override void Copy(Model m)
    {
        Route r = (Route)m;

        this.route = r.route;
    }
}
