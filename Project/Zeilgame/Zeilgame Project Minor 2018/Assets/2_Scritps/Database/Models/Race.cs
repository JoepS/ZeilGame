using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

[Table("Race")]
public class Race : Model {
    
    [PrimaryKey, NotNull, AutoIncrement]
    public int id { get; set; }
    public string Name { get; set; }
    public int BoatsUsedId { get; set; }
    public int LocationId { get; set; }
    public int TrackId { get; set; }
    public int Difficulty { get; set; }

    public override string ToString()
    {
        return id + " / " + Name + " / " + BoatsUsedId + " / " + LocationId + " / " + TrackId + " / " + Difficulty;
    }
}
