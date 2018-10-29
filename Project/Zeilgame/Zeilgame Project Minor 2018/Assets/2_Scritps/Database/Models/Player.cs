using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Table("player")]
public class Player : Model {

    [PrimaryKey, AutoIncrement, NotNull]
    public int id { get; set; }
    public string Name { get; set; }
    public float Gold { get; set; }
    public string StartLocation { get; set; }
    public int CurrentLocation { get; set; }
    public float CurrentLocationLat { get; set; }
    public float CurrentLocationLon { get; set; }
    public int CurrentRoute { get; set; }
    public string LastInGame { get; set; }
    DateTime _lastInGame;

    Boat _boat;
    Location _location;

    public Vector2 getCurrentLocationLatLon()
    {
        return new Vector2(CurrentLocationLat, CurrentLocationLon);
    }

    public void setCurrentLocationLatLon(Vector2 latlon)
    {
        CurrentLocationLat = latlon.x;
        CurrentLocationLon = latlon.y;
    }

    public void SetLastInGame(string lastInGame)
    {
        LastInGame = lastInGame;
        _lastInGame = default(DateTime);
    }

    public DateTime GetLastInGame()
    {
            _lastInGame = (DateTime)JsonUtility.FromJson<JsonDateTime>(LastInGame);

        return _lastInGame;
    }

    public Boat GetActiveBoat()
    {
        if (_boat == null)
        {
            try
            {
                _boat = MainGameController.instance.databaseController.connection.Table<Boat>().Where(x => x.Active).First();
            }
            catch(Exception e)
            {
                Debug.Log(e);
                _boat = null;
            }
        }
        return _boat;
    }

    public Location GetCurrentLocation()
    {
        _location = MainGameController.instance.databaseController.connection.Table<Location>().Where(x => x.id == CurrentLocation).First();
        return _location;
    }

    public override string ToString()
    {
        return id + " / " + Name + " / " + Gold + " / " + StartLocation + " / " + CurrentLocation + " / " + CurrentRoute + " / " + GetLastInGame(); 
    }
}
