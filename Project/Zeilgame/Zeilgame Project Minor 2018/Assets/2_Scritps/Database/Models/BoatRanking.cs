using SQLite4Unity3d;

[Table("BoatRanking")]
public class BoatRanking : Model {
    [PrimaryKey, NotNull]
    public int BoatId { get; set; }
    [PrimaryKey, NotNull]
    public int PersonId { get; set; }
    public int RacesSailed { get; set; }
    public int RacesWon { get; set; }
    public float Points { get; set; }

    public override string ToString()
    {
        return BoatId + " / " + PersonId + " / " + RacesSailed + " / " + RacesWon + " / " + Points;
    }
}
