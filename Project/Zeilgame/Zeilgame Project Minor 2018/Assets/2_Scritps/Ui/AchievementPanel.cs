using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPanel : MonoBehaviour {

    [SerializeField] Text _achivementNameText;
    [SerializeField] Text _achivementDescriptionText;
    [SerializeField] Text _achivementUnlockConditionText;
    [SerializeField] Slider _progresSlider;


	public void SetData(Achievement a)
    {
        LocalizationManager lz = MainGameController.instance.localizationManager;

        _achivementNameText.text = lz.GetLocalizedValue(a.Name);
        _achivementDescriptionText.text = lz.GetLocalizedValue(a.Description);
        List<Property> properties = a.GetProperties();
        List<float> propertyValues = a.GetPropertiesAmount();
        string s = "";

        float totalPropertyValues = 0;
        float completedPropertyValue = 0;

        for (int i = 0; i < properties.Count; i++)
        {
            int propertiesAmount = Mathf.RoundToInt(properties[i].Value);
            totalPropertyValues += propertyValues[i];
            completedPropertyValue += propertiesAmount;
            if (propertiesAmount > propertyValues[i])
                propertiesAmount = Mathf.RoundToInt(propertyValues[i]);
            s += lz.GetLocalizedValue(properties[i].Name) + ": " + propertiesAmount + " / " + propertyValues[i] + "\n";
        }
        
        _progresSlider.value = (completedPropertyValue / totalPropertyValues);
        _achivementUnlockConditionText.text = s;
    }
}
