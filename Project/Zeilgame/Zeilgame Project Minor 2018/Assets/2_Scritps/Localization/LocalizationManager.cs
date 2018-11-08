using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class LocalizationManager : MonoBehaviour
{
    private Dictionary<string, string> localizedText;
    private bool isReady = false;
    private string missingTextString = "???";

    const string _languageKey = "Language";
    public static string LanguageKey { get { return _languageKey; } }

    List<string> _languages = new List<string>() { "Nederlands", "English"};
    public List<string> Languages { get { return _languages; } }

    string _currentLanguage;
    public string CurrentLanguage { get { return _currentLanguage; } }

    void Start()
    {
        if (!PlayerPrefs.HasKey(_languageKey))
        {
            PlayerPrefs.SetString(_languageKey, "English");
            _currentLanguage = "English";
        }
        LoadLocalizedText(PlayerPrefs.GetString(_languageKey) + ".json");
    }

    public void LoadLocalizedText(string fileName)
    {
        localizedText = new Dictionary<string, string>();
//#if UNITY_EDITOR
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
//#elif UNITY_ANDROID
//        string filePath = Path.Combine ("jar:file://" + Application.dataPath + "!assets/", fileName);
//#endif
        //if (File.Exists(filePath))
        //{
#if UNITY_EDITOR
            string dataAsJson = File.ReadAllText(filePath);
#elif UNITY_ANDROID
            WWW reader = new WWW (filePath);
            while (!reader.isDone) {
            }
            string dataAsJson = reader.text;
#endif
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
#if UNITY_EDITOR
        Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
#endif
        _currentLanguage = fileName.Replace(".json", "");
        isReady = true;
    }

    public string GetLocalizedValue(string key)
    {
        string result = missingTextString + "/" + key;
        if (localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }

        return result;

    }

    public bool GetIsReady()
    {
        return isReady;
    }

}
