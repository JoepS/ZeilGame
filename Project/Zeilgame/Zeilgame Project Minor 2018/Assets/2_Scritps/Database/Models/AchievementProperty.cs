using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

[Table("AchievementProperty")]
public class AchievementProperty : Model {
    [PrimaryKey, NotNull, AutoIncrement]
    public int id { get; set; }
    public string Name { get; set; }
    public float Value { get; set; }

    public override string ToString()
    {
        return id + " / " + Name + " / " + Value;
    }

    public override void Reset()
    {
        this.Value = 0;
        this.Save();
    }
}
