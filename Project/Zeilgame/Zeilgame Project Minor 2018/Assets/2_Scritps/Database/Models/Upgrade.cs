using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

[Table("Upgrade")]
public class Upgrade : Model {

	[PrimaryKey, AutoIncrement, NotNull]
    public int id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public bool Bought { get; set; }
    public float HeightModifier { get; set; }
    public float SpeedModifier { get; set; }
    public float DamageModifier { get; set; }
    public float OfflineSpeedModifier { get; set; }


    public override string ToString()
    {
        return id + " / " + Name + " / " + Price + " / " + Bought + " / " + HeightModifier + " / " + SpeedModifier + " / " + DamageModifier + " / " + OfflineSpeedModifier;
    }

    public override void Reset()
    {
        this.Bought = false;
        this.Save();
    }

}
