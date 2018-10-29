using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System.Linq;

[Table("Achievement")]
public class Achievement : Model {
    [PrimaryKey, NotNull, AutoIncrement]
    public int id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Hidden { get; set; }
    public bool Unlocked { get; set; }
    public string Property { get; set; }
    public string PropertyAmount { get; set; }

    public override string ToString()
    {
        return id + " / " + Name + " / " + Description + " / " + Hidden + " / " + Unlocked + " / " + Property + " / " + PropertyAmount;
    }

    public List<AchievementProperty> GetProperties()
    {
        ListInt temp = JsonUtility.FromJson<ListInt>(Property);
        List<AchievementProperty> achievementProperty = MainGameController.instance.databaseController.connection.Table<AchievementProperty>().Where(x => temp.list.Contains(x.id)).ToList();
        return achievementProperty;
    }

    public List<float> GetPropertiesAmount()
    {
        ListFloat temp = JsonUtility.FromJson<ListFloat>(PropertyAmount);
        return temp.list;
    }

    public override void Reset()
    {
        this.Unlocked = false;
        this.Save();
    }

}
