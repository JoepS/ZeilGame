using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RequestProperty : Model {
    
    public string Name { get; set; }
    public string Value { get; set; }

    public override void Copy(Model m)
    {
        RequestProperty rp = (RequestProperty)m;
        this.Name = rp.Name;
        this.Value = rp.Value;

    }

    public List<string> GetKeys()
    {
        return JsonUtility.FromJson<ListString>(Value).list;
    }

    public override string ToString()
    {
        return Name + " / " + Value;
    }
}

public class PropertyValue
{
    public Dictionary<string, object> dictionary;

    public PropertyValue(LitJson.JsonData data)
    {
        dictionary = new Dictionary<string, object>();
        foreach(string key in data.Keys)
        {
            if (data[key].IsBoolean)
            {
                dictionary.Add(key, (bool)data[key]);
            }
            else if (data[key].IsInt)
            {
                dictionary.Add(key, (int)data[key]);
            }
            else if (data[key].IsString)
            {
                dictionary.Add(key, (string)data[key]);
            }
            else if (data[key].IsObject)
            {
                dictionary.Add(key, (object)data[key]);
            }
        }
    }

    public object GetValue(string key)
    {
        object obj = null;
        dictionary.TryGetValue(key, out obj);
        if (obj == null)
            return obj;
        switch (key)
        {
            case "boat":
                int id = (int)(dictionary[key]);
                return id;// MainGameController.instance.databaseController.connection.Table<Boat>().Where(x => x.id == id).First().Name;
            case "location":
                int idL = (int)(dictionary[key]);
                return idL;//MainGameController.instance.databaseController.connection.Table<Location>().Where(x => x.id == idL).First().Name;
            case "time":
                return dictionary[key];
            case "item":
                int idI = (int)(dictionary[key]);
                return idI;//MainGameController.instance.databaseController.connection.Table<Item>().Where(x => x.id == idI).First().Name;
            case "race":
                int idR = (int)(dictionary[key]);
                return idR;//MainGameController.instance.databaseController.connection.Table<Race>().Where(x => x.id == idR).First().Name;
            default:
                return "Unknown";
        }
    }

    public override string ToString()
    {
        string s = "";
        foreach (string key in dictionary.Keys)
        {
            s += dictionary[key] + " / ";
        }
        return s;
    }
}
