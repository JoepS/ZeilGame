using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Request : Model {
    public string Name { get; set; }
    public string Description { get; set; }
    public int LocationId { get; set; }
    public bool Accepted { get; set; }
    public bool Completed { get; set; }
    public string RewardType { get; set; }
    public int Reward { get; set; }
    public int Property { get; set; }
    public string PropertyValues { get; set; }

    PropertyValue _propertyValue;

    public RequestProperty GetProperty()
    {
        return MainGameController.instance.databaseController.connection.Table<RequestProperty>().Where(x => x.id == Property).First();
    }

    public PropertyValue GetPropertyValues()
    {
        if (_propertyValue == null)
        {
            List<string> listString = JsonUtility.FromJson<ListString>(GetProperty().Value).list;
            List<int> listInt = JsonUtility.FromJson<ListInt>(PropertyValues).list;
            string json = "{";
            for (int i = 0; i < listString.Count; i++)
            {
                json += "\"" + listString[i] + "\":" + listInt[i] + ",";
            }
            json = json.Remove(json.Length - 1);
            json += "}";
            JsonData data = JsonMapper.ToObject(json);
            PropertyValue pv = new PropertyValue(data);
            _propertyValue = pv;
        }
        return _propertyValue;
    }

    public System.Object GetReward()
    {
        switch (RewardType)
        {
            case "gold_text":
                return Reward;
            case "boat_text":
                return MainGameController.instance.databaseController.connection.Table<Boat>().Where(x => x.id == Reward).First();
            case "upgrade_text":
                return MainGameController.instance.databaseController.connection.Table<Upgrade>().Where(x => x.id == Reward).First();
            case "sail_text":
                return MainGameController.instance.databaseController.connection.Table<Sail>().Where(x => x.id == Reward).First();
            case "item_text":
                return MainGameController.instance.databaseController.connection.Table<Item>().Where(x => x.id == Reward).First();
            default:
                return "Unknown";
        }
    }

    public override void Reset()
    {
        this.Accepted = false;
        this.Completed = false;
        this.Save();
    }

    public override void Copy(Model m)
    {
        Request r = (Request)m;
        this.Name = r.Name;
        this.Description = r.Description;
        this.LocationId = r.LocationId;
        this.RewardType = r.RewardType;
        this.Reward = r.Reward;
        this.Property = r.Property;
        this.PropertyValues = r.PropertyValues;
    }

    public override string ToString()
    {
        return Name + " / " + Description + " / " + LocationId + " / " + Accepted + " / " + Completed + " / " + RewardType + " / " + Reward + " / " + Property + " / " + PropertyValues;
    }
}
