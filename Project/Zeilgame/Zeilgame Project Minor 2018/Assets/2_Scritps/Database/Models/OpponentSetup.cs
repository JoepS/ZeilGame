using SQLite4Unity3d;

[Table("OpponentSetup")]
public class OpponentSetup : Model {
    public float TackingAccuracy { get; set; }
    public float TackingExtra { get; set; }
    public float TackingSpeed { get; set; }
    public float BestUpwindAngle { get; set; }

    public override string ToString()
    {
        return id + " / " + TackingAccuracy + " / " + TackingExtra + " / " + TackingSpeed + " / " + BestUpwindAngle;
    }

    public override void Copy(Model m)
    {
        OpponentSetup o = (OpponentSetup)m;

        this.TackingAccuracy = o.TackingAccuracy;
        this.TackingExtra = o.TackingExtra;
        this.TackingSpeed = o.TackingSpeed;
        this.BestUpwindAngle = o.BestUpwindAngle;
    }
}
