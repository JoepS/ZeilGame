using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class RequestManager {
    
    public static void CompleteRequest(Request r)
    {
        r.Completed = true;

        object o = r.GetReward();
        string rewardType = MainGameController.instance.localizationManager.GetLocalizedValue(r.RewardType);
        string athing = MainGameController.instance.localizationManager.GetLocalizedValue("a_text");
        switch (o.GetType().Name)
        {
            case "Int32":
                int reward = (int)o;
                MainGameController.instance.player.GiveGold(reward);
                break;
            case "Boat":
                Boat b = (Boat)o;
                b.Bought = true;
                b.Save();
                rewardType = athing + " " + rewardType; 
                break;
            case "Sail":
                Sail s = (Sail)o;
                s.Bought = true;
                s.Save();
                rewardType = athing + " " + rewardType;
                break;
            case "Upgrade":
                Upgrade u = (Upgrade)o;
                u.Bought = true;
                u.Save();
                rewardType = athing + " " + rewardType;
                break;
            case "Item":
                Item i = (Item)o;
                i.InInventory++;
                i.Save();
                rewardType = athing + " " + rewardType;
                break;
            default:
                Debug.LogWarning("Unknown reward type: " + o.GetType().Name);
                break;
        }

        r.Save();
        string popupText = string.Format(MainGameController.instance.localizationManager.GetLocalizedValue("completed_request_text"), rewardType);

        MainGameController.instance.popupManager.ViewPopup(popupText, null, 10);
    }

    public static void UpdateRequest(params object[] objs)
    {
        List<string> objTypes = new List<string>();
        List<int> objValues = new List<int>();

        foreach(object o in objs)
        {
            switch(o.GetType().Name)
            {
                case "Boat":
                    objTypes.Add("boat");
                    objValues.Add(((Boat)(o)).id);
                    break;
                case "Location":
                    objTypes.Add("location");
                    objValues.Add(((Location)(o)).id);
                    break;
                case "Race":
                    objTypes.Add("race");
                    objValues.Add(((Race)(o)).id);
                    break;
                case "Item":
                    objTypes.Add("item");
                    objValues.Add(((Item)(o)).id);
                    break;
                case "Int32":
                    objTypes.Add("time");
                    objValues.Add((int)(o));
                    break;
                default:
                    Debug.LogWarning("Unknown Type: " + o.GetType().Name);
                    break;
            }
        }

        List<Request> acceptedRequests = MainGameController.instance.databaseController.connection.Table<Request>().Where(x => x.Accepted && x.Completed == false).ToList();
        foreach(Request r in acceptedRequests)
        {
            List<string> keys = r.GetProperty().GetKeys().OrderBy(x => x).ToList();

            if (objTypes.Count.Equals(keys.Count)){
                bool same = true;
                for(int i = 0; i < objTypes.Count; i++)
                {
                    if(objTypes[i] != keys[i])
                    {
                        same = false;
                        break;
                    }
                }
                if (same)
                {
                    bool sameValues = true;
                    for (int i = 0; i < objValues.Count; i++)
                    {
                        if (objValues[i] != (int)r.GetPropertyValues().GetValue(objTypes[i])){
                            sameValues = false;
                            break;
                        }
                    }
                    if (sameValues)
                    {
                        CompleteRequest(r);
                    }
                }
            }
        }
    }

}
