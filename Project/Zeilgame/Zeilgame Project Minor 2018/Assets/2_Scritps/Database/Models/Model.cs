using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

public class Model {
    [PrimaryKey, NotNull]
    public int id { get; set; }

    public void New()
    {
        MainGameController.instance.databaseController.connection.Insert(this);
    }

	public void Save()
    {
        MainGameController.instance.databaseController.connection.Update(this);
    }

    public void Delete()
    {
        MainGameController.instance.databaseController.connection.Delete(this);
    }

    public virtual void Reset()
    {
        throw new System.Exception("No reset function definend!");
    }

    public virtual void Copy(Model m)
    {
        throw new System.Exception("No copy function defined!");
    }
}
