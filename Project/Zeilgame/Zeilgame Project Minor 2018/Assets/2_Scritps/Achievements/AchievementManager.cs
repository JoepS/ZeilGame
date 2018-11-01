using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AchievementManager : MonoBehaviour {

    List<Achievement> _achievements;
    List<Property> _properties;

	// Use this for initialization
	void Start () {
        _achievements = MainGameController.instance.databaseController.connection.Table<Achievement>().ToList();
        _properties = MainGameController.instance.databaseController.connection.Table<Property>().ToList();
	}

    void UnlockAchievement(Achievement a)
    {
        a.Unlocked = true;
        a.Hidden = false;
        MainGameController.instance.popupManager.ViewPopup("Achivement Unlocked!\n" + MainGameController.instance.localizationManager.GetLocalizedValue(a.Name), null, 10);
        a.Save();
    }

    public void AddAchievementProperty(AchievementProperties property, float amount)
    {
        int propertyId = (int)property;
        Property ap = _properties.Where(x => x.id.Equals(propertyId)).First();
        ap.Value += amount;
        ap.Save();
        CheckForAchievementUnlocked();
    }

    public void CheckForAchievementUnlocked()
    {
        Reset();
        foreach(Achievement a in _achievements.Where(x=>!x.Unlocked))
        {
            List<Property> properties = a.GetProperties();
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

    public void Reset()
    {
        _achievements = MainGameController.instance.databaseController.connection.Table<Achievement>().ToList();
        _properties = MainGameController.instance.databaseController.connection.Table<Property>().ToList();
    }
}


public enum AchievementProperties
{
    Distance = 1,
    Races = 2,
    RacesWon = 3,
    BoatsBought = 4,
    CratesOpened = 5
}

