[System.Serializable]
public class LocalizationData
{
    public string name;
    public LocalizationItem[] items;
}

[System.Serializable]
public class LocalizationItem
{
    public string key;
    public string value;
}