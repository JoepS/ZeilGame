using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopupPanel : MonoBehaviour, IPointerClickHandler {

    [SerializeField] Text _popupText;
    [SerializeField] Image _popupImage;

    PopupData _data;

    CanvasGroup _canvasGroup;
    bool _clicked = false;

    float _remainingViewTime;
    public float remainingViewTime { get { return _remainingViewTime; } }
    
    void Start()
    {
        _canvasGroup = this.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        StartCoroutine(Fade(_data.viewTime));
    }

    public void SetData(PopupData data)
    {
        _data = data;
        _popupText.text = data.text;
        _popupImage.sprite = data.image;
    }

    IEnumerator Fade(float time)
    {
        while(_canvasGroup.alpha < 1)
        {
            _canvasGroup.alpha += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        _canvasGroup.alpha = 1;
        float timeElapsed = 0;
        while(timeElapsed < time && !_clicked)
        {
            yield return new WaitForSeconds(1);
            timeElapsed += 1;
            _remainingViewTime = time - timeElapsed;
        }
        float alpha = _canvasGroup.alpha;
        while(alpha >= 0)
        {
            alpha -= 0.1f;
            _canvasGroup.alpha = alpha;
            yield return new WaitForSeconds(0.1f);
        }
        MainGameController.instance.popupManager.RemoveDataFromList(this._data);
        Destroy(this.gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _clicked = true;
    }
}
