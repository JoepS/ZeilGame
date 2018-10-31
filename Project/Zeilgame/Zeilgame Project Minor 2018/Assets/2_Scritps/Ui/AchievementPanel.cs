using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPanel : MonoBehaviour {

    [SerializeField] Text _achivementNameText;
    [SerializeField] Text _achivementDescriptionText;
    [SerializeField] Text _achivementUnlockConditionText;


	public void SetData(Achievement a)
    {
        LocalizationManager lz = MainGameController.instance.localizationManager;

        _achivementNameText.text = lz.GetLocalizedValue(a.Name);
        _achivementDescriptionText.text = lz.GetLocalizedValue(a.Description);
        List<AchievementProperty> properties = a.GetProperties();
        List<float> propertyValues = a.GetPropertiesAmount();
        string s = "";

        for (int i = 0; i < properties.Count; i++)
        {
            int propertiesAmount = Mathf.RoundToInt(properties[i].Value);
            if (propertiesAmount > propertyValues[i])
                propertiesAmount = Mathf.RoundToInt(propertyValues[i]);
            s += lz.GetLocalizedValue(properties[i].Name) + ": " + propertiesAmount + " / " + propertyValues[i] + "\n";
        }

        _achivementUnlockConditionText.text = s;
    }
}
