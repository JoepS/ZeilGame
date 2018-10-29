using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

[Table("Version")]
public class Version : Model {
    [PrimaryKey, NotNull]
    public int id { get; set; }
    public int versionId { get; set; }
}
