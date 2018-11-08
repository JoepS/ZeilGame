using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RequestManager {
    
    public static void CompleteRequest(Request r)
    {
        r.Completed = true;
        r.Save();
    }

    public static void UpdateRequest(params object[] objs)
    {
        List<string> objTypes = new List<string>();
        List<int> objValues = new List<int>();

        foreach(object o in objs)
        {
            switch(o.GetType().ToString())
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
                default:
                    objTypes.Add("time");
                    objValues.Add((int)(o));
                    break;
            }
        }
        
        for(int i = 0; i < objTypes.Count; i++)
        {
            Debug.Log(objTypes[i] + " / " + objValues[i]);
        }
    }

}
