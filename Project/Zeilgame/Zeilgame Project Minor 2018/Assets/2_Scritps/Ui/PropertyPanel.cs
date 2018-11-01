using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyPanel : MonoBehaviour {

    [SerializeField] Text _propertyNameText;
    [SerializeField] Text _propertyValueText;

	public void SetData(Property p)
    {
        _propertyNameText.text = MainGameController.instance.localizationManager.GetLocalizedValue(p.Name) + ": ";
        _propertyValueText.text = p.Value + "";
    }
}
