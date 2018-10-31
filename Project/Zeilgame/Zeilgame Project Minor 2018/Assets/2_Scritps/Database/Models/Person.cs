using SQLite4Unity3d;

[Table("Person")]
public class Person : Model {
    public string Name { get; set; }
    public int BoatId { get; set; }
    public int LocationId { get; set; }
    public int RacesSailed { get; set; }
    public int RacesWon { get; set; }
    public float LifetimePoints { get; set; }
    public int OpponentSetupId { get; set; }

    public override string ToString()
    {
        return id + " / " + Name + " / " + BoatId + " / " + LocationId + " / " + RacesSailed + " / " + RacesWon + " / " + LifetimePoints;
    }

    public override void Reset()
    {
        this.RacesSailed = 0;
        this.RacesWon = 0;
        this.LifetimePoints = 0;
        this.Save();
    }

    public override void Copy(Model m)
    {
        Person p = (Person)m;

        this.Name = p.Name;
        this.BoatId = p.BoatId;
        this.LocationId = p.LocationId;
        this.OpponentSetupId = p.OpponentSetupId;
    }
}
