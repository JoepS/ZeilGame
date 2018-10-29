using SQLite4Unity3d;

[Table("OpponentSetup")]
public class OpponentSetup : Model {
    [PrimaryKey, NotNull, AutoIncrement]
    public int id { get; set; }
    public float TackingAccuracy { get; set; }
    public float TackingExtra { get; set; }
    public float TackingSpeed { get; set; }
    public float BestUpwindAngle { get; set; }

    public override string ToString()
    {
        return id + " / " + TackingAccuracy + " / " + TackingExtra + " / " + TackingSpeed + " / " + BestUpwindAngle;
    }
}
