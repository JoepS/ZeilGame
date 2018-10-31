using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

[Table("Race")]
public class Race : Model {
    
    public string Name { get; set; }
    public int BoatsUsedId { get; set; }
    public int LocationId { get; set; }
    public int TrackId { get; set; }
    public int Difficulty { get; set; }

    public override string ToString()
    {
        return id + " / " + Name + " / " + BoatsUsedId + " / " + LocationId + " / " + TrackId + " / " + Difficulty;
    }

    public override void Copy(Model m)
    {
        Race r = (Race)m;

        this.Name = r.Name;
        this.BoatsUsedId = r.BoatsUsedId;
        this.Difficulty = r.Difficulty;
        this.LocationId = r.LocationId;
        this.TrackId = r.TrackId;
    }
}
