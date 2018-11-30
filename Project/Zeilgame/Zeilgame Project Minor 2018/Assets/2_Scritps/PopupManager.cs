using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopupManager : MonoBehaviour {

    [SerializeField] GameObject _popupPanelParentPrefab;
    GameObject _popupPanelParent;
    [SerializeField] GameObject _popupPrefab;

    static int tempIndex = 0;

    static List<PopupData> _popupDatas;

    void Awake()
    {
        if (_popupDatas == null)
        {
            _popupDatas = new List<PopupData>();
        }
        GameObject canvas = GameObject.Find("Canvas");
        _popupPanelParent = null;
        _popupPanelParent = GameObject.Instantiate(_popupPanelParentPrefab);
        _popupPanelParent.name = "PopupPanels" + tempIndex;
        tempIndex++;
        RectTransform rt = _popupPanelParent.GetComponent<RectTransform>();
        _popupPanelParent.transform.SetParent(canvas.transform);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchorMin = new Vector2(0, 0);
        rt.offsetMin = new Vector2(0, 0);
        rt.offsetMax = new Vector2(0, 0);
        _popupPanelParent.transform.localPosition = Vector3.zero;
        _popupPanelParent.transform.localScale = Vector3.one;
        
        foreach(PopupData pd in _popupDatas)
        {
            if (pd.assignedToGameObject == null && pd.remainingTime >= 1) {
                ViewPopup(pd);
            }
        }
       
    }

    public void Reset()
    {
        _popupPanelParent = GameObject.Find("PopupPanels" + (tempIndex-1));
    }

    public void ViewPopup(PopupData data)
    {
        if (_popupPanelParent == null)
            Reset();
        CreatePopup(data);
    }

    public void ViewPopup(string text, Sprite image, float viewTime)
    {
        if (_popupDatas.Count > 0)
        {
            if (_popupDatas.Where(x => x.text.Equals(text)).FirstOrDefault() != null)
            {
                _popupDatas.Where(x => x.text.Equals(text)).First().assignedToGameObject.GetComponent<PopupPanel>().Reset();
                return;
            }
        }
        PopupData data = new PopupData(text, image, viewTime);
        _popupDatas.Add(data);
        if (_popupPanelParent == null)
        {
            Reset();
        }
        CreatePopup(data);
    }

    void CreatePopup(PopupData data)
    {
        GameObject popup = data.assignedToGameObject;
        if(popup == null)
            popup = GameObject.Instantiate(_popupPrefab);
        popup.transform.SetParent(_popupPanelParent.transform);
        popup.transform.localScale = Vector3.one;
        popup.GetComponent<PopupPanel>().SetData(data);
        data.assignedToGameObject = popup;
        Vector2 offsetMax = popup.GetComponent<RectTransform>().offsetMax;
        offsetMax.x = 0;
        popup.GetComponent<RectTransform>().offsetMax = offsetMax;
    }

    public void RemoveDataFromList(PopupData data)
    {
        _popupDatas.Remove(data);
    }
}
