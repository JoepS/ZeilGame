﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SQLite4Unity3d;

[Table("Boat")]
public class Boat : Model {

    [PrimaryKey, AutoIncrement, NotNull]
    public int id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public bool Bought { get; set; }
    public bool Active { get; set; }
    public float MaxDamage { get; set; }
    public float Damage { get; set; }
    public string Upgrades { get; set; }
    ListInt _upgradesId;
    public string Sails { get; set; }
    ListInt _sailsId;
    public float HullLength { get; set; }
    public float OfflineSpeed { get; set; }
    public float NGZLimit { get; set; }

    List<Upgrade> _boatUpgrades;
    List<Sail> _sails;

    public Boat()
    {

    }

    public override void Reset()
    {
        this.Active = false;
        this.Bought = false;
        this.Damage = 0;
        this.Save();
    }

    public List<int> GetUpgradesId()
    {
        if (_upgradesId == null)
            _upgradesId = JsonUtility.FromJson<ListInt>(Upgrades);

        return _upgradesId.list;
    }

    public List<Upgrade> GetUpgrades()
    {
        _boatUpgrades = new List<Upgrade>();
        foreach(int i in GetUpgradesId())
        {
            Upgrade u = MainGameController.instance.databaseController.connection.Table<Upgrade>().Where(x => x.id == i).First();
            if(u.Bought)
                _boatUpgrades.Add(u);
        }
        return _boatUpgrades;
    }

    public List<int> GetSailsId()
    {
        if (_sailsId == null)
            _sailsId = JsonUtility.FromJson<ListInt>(Sails);

        return _sailsId.list;
    }

    public List<Sail> GetSails()
    {
        _sails = new List<Sail>();
        foreach(int i in GetSailsId())
        {
            Sail s = MainGameController.instance.databaseController.connection.Table<Sail>().Where(x => x.id == i).First();
            if (s.Bought)
                _sails.Add(s);
        }
        return _sails;
    }

    public float GetMaxDamage()
    {
        float damageModifier = 1;
        foreach(Upgrade u in GetUpgrades())
        {
            damageModifier *= u.DamageModifier;
        }

        foreach(Sail s in GetSails())
        {
            damageModifier *= s.DamageModifier;
        }
        return MaxDamage * damageModifier;
    }

    public float GetNGZLimit()
    {
        return NGZLimit;
    }

    public float GetNGZLimitModified()
    {
        float NGZLimitModifier = 1;
        foreach(Upgrade u in GetUpgrades())
        {
            NGZLimitModifier *= u.HeightModifier;
        }
        return NGZLimit * NGZLimitModifier;
    }

    public float GetHullLenght()
    {
        float hullLengthModifier = 1;
        foreach(Upgrade u in GetUpgrades())
        {
            hullLengthModifier *= u.SpeedModifier;
        }
        foreach(Sail s in GetSails())
        {
            hullLengthModifier *= s.SpeedModifier;
        }
        return HullLength * hullLengthModifier;
    }

    public float GetOfflineSpeed()
    {
        float offlineSpeedModifier = 1;
        foreach(Upgrade u in GetUpgrades())
        {
            offlineSpeedModifier *= u.OfflineSpeedModifier;
        }
        foreach(Sail s in GetSails())
        {
            offlineSpeedModifier *= s.OfflineSpeedModifier;
        }
        return OfflineSpeed * offlineSpeedModifier;
    }

    public float GetSpeedModified()
    {
        float speed = 4.49f * Mathf.Sqrt(GetHullLenght());
        float damageModifier = 1;
        damageModifier -= (Damage / GetMaxDamage());
       // if (damageModifier == 0)
       //     damageModifier = 0.05f;

        speed *= damageModifier;
        return speed;
    }

    public float GetSpeed()
    {
        float speed = 4.49f * Mathf.Sqrt(HullLength); 
        float damageModifier = 1;
        damageModifier -= (Damage / GetMaxDamage());
        //if (damageModifier == 0)
       //     damageModifier = 0.05f;

        speed *= damageModifier;
        return speed;
    }

    public float GetMaxWindSpeedSails()
    {
        if (GetSails().Where(x => x.Active).Count() > 0)
        {
            Sail s = GetSails().Where(x => x.Active).First();
            return s.MaxWindSpeed;
        }
        else
            return GetSpeedModified();
    }

    public override string ToString()
    {
        return id + " / " + Name + " / " + Price + " / " + Bought + " / " + Active + " / " + Upgrades + " / " + Sails + " / " + OfflineSpeed;
    }
}