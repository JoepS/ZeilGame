using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyPanel : MonoBehaviour {

    [SerializeField] Text _propertyNameText;
    [SerializeField] Text _propertyValueText;

	public void SetData(string name, object value)
    {
        _propertyNameText.text = MainGameController.instance.localizationManager.GetLocalizedValue(name) + ": ";
        _propertyValueText.text = value + "";
    }
}
