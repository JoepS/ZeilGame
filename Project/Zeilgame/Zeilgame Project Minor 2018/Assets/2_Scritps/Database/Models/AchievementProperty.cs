using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

[Table("AchievementProperty")]
public class AchievementProperty : Model {
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

    public override void Copy(Model m)
    {
        AchievementProperty ap = (AchievementProperty)m;
        this.Name = ap.Name;
    }
}
