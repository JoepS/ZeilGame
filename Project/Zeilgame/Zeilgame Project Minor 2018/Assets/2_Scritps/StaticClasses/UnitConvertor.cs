using UnityEngine;

public static class UnitConvertor {

    const string _unitsKey = "UNITS";
    public static string UnitOptionKey { get { return _unitsKey; } }

    public static float Convert(float value)
    {
        switch (PlayerPrefs.GetString(_unitsKey))
        {
            case "Imperial":
                return value / 1.609f;
            case "Metric":
                return value;
        }
        return value;
    }

    public static string UnitString()
    {
        switch (PlayerPrefs.GetString(_unitsKey))
        {
            case "Imperial":
                return " M";
            case "Metric":
                return " Km";
        }
        return "Km";
    }
}
