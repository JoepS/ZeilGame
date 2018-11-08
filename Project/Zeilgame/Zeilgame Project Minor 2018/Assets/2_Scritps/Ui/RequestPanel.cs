using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RequestPanel : MonoBehaviour {

    [SerializeField] Text _nameText;
    [SerializeField] Text _descriptionText;
    [SerializeField] Text _rewardText;
    [SerializeField] GameObject _propertyPanel;
    [SerializeField] GameObject _propertyPanelPrefab;

    Color _avaliableColor = Color.blue;
    Color _acceptedColor = Color.yellow;
    Color _completedColor = Color.green;

    Request _request;

    UnityAction _action;

    public void SetData(Request r, UnityAction actionOnClick)
    {
        _request = r;
        _action = actionOnClick;
        _nameText.text = MainGameController.instance.localizationManager.GetLocalizedValue(r.Name);
        _descriptionText.text = MainGameController.instance.localizationManager.GetLocalizedValue(r.Description);
        _rewardText.text = MainGameController.instance.localizationManager.GetLocalizedValue(r.RewardType) + ": " + r.GetReward();
        foreach (string key in r.GetProperty().GetKeys())
        {
            GameObject go = GameObject.Instantiate(_propertyPanelPrefab, _propertyPanel.transform);
            go.transform.localScale = Vector3.one;
            go.GetComponent<PropertyPanel>().SetData(key + "_text", r.GetPropertyValues().GetValue(key));
        }

        Color c;

        if (r.Completed)
            c = _completedColor;
        else if (r.Accepted)
            c = _acceptedColor;
        else
            c = _avaliableColor;

        c.a = 0.5f;
        this.GetComponent<Image>().color = c;
    }

    public void OnMouseDown()
    {
        if(!_request.Accepted)
        {
            _request.Accepted = true;
            _request.Save();
            Color c = _acceptedColor;
            c.a = .5f;
            this.GetComponent<Image>().color = c;
        }
        _action.Invoke();
    }
}
