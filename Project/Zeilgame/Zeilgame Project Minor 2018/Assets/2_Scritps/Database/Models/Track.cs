using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

[Table("Track")]
public class Track : Model {
    
    public string Name { get; set; }
    public string Waypoints { get; set; }
    public string WaypointOrder { get; set; }
    public ListV2 _waypoints;
    public ListInt _waypointOrder;

    public List<Vector2> GetWaypoints()
    {
        if (_waypoints == null)
            _waypoints = JsonUtility.FromJson<ListV2>(Waypoints);
        return _waypoints.list;
    }

    public List<int> GetWaypointOrder()
    {
        if (_waypointOrder == null)
            _waypointOrder = JsonUtility.FromJson<ListInt>(WaypointOrder);
        return _waypointOrder.list;
    }

    public override string ToString()
    {
        return id + " / " + Name + " / " + Waypoints + " / " + WaypointOrder;
    }

    public override void Copy(Model m)
    {
        Track t = (Track)m;

        this.Name = t.Name;
        this.Waypoints = t.Waypoints;
        this.WaypointOrder = t.WaypointOrder;
    }
}
