using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

[Table("Sail")]
public class Sail : Model {

    public string Name { get; set; }
    public int Price { get; set; }
    public bool Bought { get; set; }
    public bool Active { get; set; }
    public float MaxWindSpeed { get; set; }
    public float DamageModifier { get; set; }
    public float SpeedModifier { get; set; }
    public float OfflineSpeedModifier { get; set; }

    public override string ToString()
    {
        return id + " / " + Name + " / " + Price + " / " + Bought + " / " + Active;
    }

    public override void Reset()
    {
        this.Bought = false;
        this.Active = false;
        this.Save();
    }

    public override void Copy(Model m)
    {
        Sail s = (Sail)m;

        this.Name = s.Name;
        this.Price = s.Price;
        this.MaxWindSpeed = s.MaxWindSpeed;
        this.DamageModifier = s.DamageModifier;
        this.SpeedModifier = s.SpeedModifier;
        this.OfflineSpeedModifier = s.OfflineSpeedModifier;
    }

}
