﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AchievementManager : MonoBehaviour {

    List<Achievement> _achievements;
    List<AchievementProperty> _properties;

	// Use this for initialization
	void Start () {
        _achievements = MainGameController.instance.databaseController.connection.Table<Achievement>().ToList();
        _properties = MainGameController.instance.databaseController.connection.Table<AchievementProperty>().ToList();
	}

    void UnlockAchievement(Achievement a)
    {
        a.Unlocked = true;
        a.Hidden = false;
        Debug.Log("Unlocked achievement: " + a);

        MainGameController.instance.popupManager.ViewPopup("Achivement Unlocked!\n" + a, null, 10);
        a.Save();
    }

    public void AddAchievementProperty(string name, float amount)
    {
        AchievementProperty ap = _properties.Where(x => x.Name.Equals(name)).First();
        ap.Value += amount;
        ap.Save();
        CheckForAchievementUnlocked();
    }

    public void CheckForAchievementUnlocked()
    {
        foreach(Achievement a in _achievements.Where(x=>!x.Unlocked))
        {
            List<AchievementProperty> properties = a.GetProperties();
            List<float> propertiesAmount = a.GetPropertiesAmount();
            bool unlocked = true;
            for (int i = 0; i < properties.Count; i++)
            {
                if(properties[i].Value < propertiesAmount[i])
                {
                    unlocked = false;
                }
            }
            if (unlocked)
            {
                UnlockAchievement(a);
            }
        }
    }
}


public enum AchievementProperties
{
    Distance,
    Races,
    RacesWon
}
