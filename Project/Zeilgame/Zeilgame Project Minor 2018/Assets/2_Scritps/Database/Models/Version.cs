using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

[Table("Version")]
public class Version : Model {
    public int versionId { get; set; }

    public override void Copy(Model m)
    {
        Version v = (Version)m;

        this.versionId = v.versionId;
    }
}
