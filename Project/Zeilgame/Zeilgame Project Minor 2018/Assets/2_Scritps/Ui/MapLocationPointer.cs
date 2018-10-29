using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLocationPointer : MonoBehaviour {

    Location _location;

    Button _button;

    [SerializeField] GameObject _popupPanel;
    [SerializeField] Text _popupText;

	// Use this for initialization
	void Start () {
        _button = this.GetComponent<Button>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetPopupText(string text)
    {
        _popupText.text = text;
    }

    public void ViewPopupPanel(bool enabled)
    {
        _popupPanel.SetActive(enabled);
    }

    public Button GetButton()
    {
        if (_button == null)
            _button = this.GetComponent<Button>();
        return _button;
    }

    public Location GetLocation()
    {
        return _location;
    }

    public void SetData(Location l)
    {
        _location = l;
    }

}
