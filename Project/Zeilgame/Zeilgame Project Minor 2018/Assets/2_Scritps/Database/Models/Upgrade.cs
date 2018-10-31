using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

[Table("Upgrade")]
public class Upgrade : Model {
    
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

    public override void Copy(Model m)
    {
        Upgrade u = (Upgrade)m;

        this.Name = u.Name;
        this.Price = u.Price;
        this.HeightModifier = u.HeightModifier;
        this.SpeedModifier = u.SpeedModifier;
        this.DamageModifier = u.DamageModifier;
        this.OfflineSpeedModifier = u.OfflineSpeedModifier;
    }

}
