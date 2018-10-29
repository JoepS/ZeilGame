using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model {

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
}
