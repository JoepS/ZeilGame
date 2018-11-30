using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Table("player")]
public class Player : Model {
    public string Name { get; set; }
    public float Gold { get; set; }
    public string StartLocation { get; set; }
    public int CurrentLocation { get; set; }
    public float CurrentLocationLat { get; set; }
    public float CurrentLocationLon { get; set; }
    public int CurrentRoute { get; set; }
    public string LastInGame { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    DateTime _lastInGame;

    Boat _boat;
    Location _location;

    const int BaseXP = 83;

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

    public void GiveGold(int amount)
    {
        Gold += amount;
        this.Save();
    }

    public DateTime GetLastInGame()
    {
            _lastInGame = (DateTime)JsonUtility.FromJson<JsonDateTime>(LastInGame);

        return _lastInGame;
    }

    public Boat GetActiveBoat()
    {
        try
        {
            _boat = MainGameController.instance.databaseController.connection.Table<Boat>().Where(x => x.Active).First();
        }
        catch
        {
            _boat = null;
        }
        return _boat;
    }

    public Location GetCurrentLocation()
    {
        _location = MainGameController.instance.databaseController.connection.Table<Location>().Where(x => x.id == CurrentLocation).First();
        return _location;
    }

    public void AddExperience(int amount)
    {
        this.Experience += amount;
        CheckIfLevelUp();
    }

    public void LevelUp()
    {
        Level++;
        Experience = 0;
        MainGameController.instance.popupManager.ViewPopup("You leveled up!\n You are now level " + Level, null, 10);
        Debug.LogWarning("You Leveled up!\nYou are now level " + Level);
    }

    public void CheckIfLevelUp()
    {
        int levelUpXp = ExperienceController.GetExperienceForLevel(Level);
        if(Experience > levelUpXp)
        {
            int remainderXp = Experience - levelUpXp;
            Debug.Log("Level Up! Remainder " + remainderXp);
            LevelUp();
            AddExperience(remainderXp);
        }
    }

    public Route GetCurrentRoute()
    {
        if(CurrentRoute != -1)
        {
            Route r = MainGameController.instance.databaseController.connection.Table<Route>().Where(x => x.id == CurrentRoute).FirstOrDefault();
            return r;
        }
        return null;
    }

    public override string ToString()
    {
        return id + " / " + Name + " / " + Gold + " / " + StartLocation + " / " + CurrentLocation + " / " + CurrentRoute + " / " + GetLastInGame() + " / " + Level + " / " + Experience;
    }
}
