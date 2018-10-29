using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

[Table("Location")]
public class Location : Model {

    [PrimaryKey, AutoIncrement, NotNull]
    public int id { get; set; }
    public string Name { get; set; }
    public float lat { get; set; }
    public float lon { get; set; }
    public string AvaliableBoats { get; set; }

    public override string ToString()
    {
        return id + " / " + Name + " / " + lat + " / " + lon;
    }

    public Vector2 GetLocation()
    {
        return new Vector2(lat, lon);
    }

    public List<int> GetAvaliableBoatsIds()
    {
        ListInt li = JsonUtility.FromJson<ListInt>(AvaliableBoats);
        return li.list;
    }

}
