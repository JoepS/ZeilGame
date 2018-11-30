using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopupPanel : MonoBehaviour, IPointerClickHandler {

    [SerializeField] Text _popupText;
    [SerializeField] Image _popupImage;
    [SerializeField] Text _timerText;

    PopupData _data;

    [SerializeField] CanvasGroup _canvasGroup;
    bool _clicked = false;

    float _remainingViewTime;
    public float remainingViewTime { get { return _remainingViewTime; } }
    
    void Start()
    {
        //_canvasGroup = this.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        float time = _data.viewTime;
        if (_data.remainingTime > 0)
        {
            time = _data.remainingTime;
            _remainingViewTime = _data.remainingTime;
        }
        StartCoroutine(Fade(time));
    }

    public void SetData(PopupData data)
    {
        _data = data;
        _popupText.text = data.text;
        _popupImage.sprite = data.image;
    }

    public void Reset()
    {
        StopAllCoroutines();
        StartCoroutine(Fade(_data.viewTime));
    }

    IEnumerator Fade(float time)
    {
        while(_canvasGroup.alpha < 1 && _remainingViewTime == 0)
        {
            _canvasGroup.alpha += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        _canvasGroup.alpha = 1;
        float timeElapsed = 0;
        while(timeElapsed < time && !_clicked)
        {
            timeElapsed += 1;
            _remainingViewTime = time - timeElapsed;
            if (_remainingViewTime == 0)
                _remainingViewTime = 0.1f;
            _timerText.text = timeElapsed + "/" + _remainingViewTime;

            _data.remainingTime = _remainingViewTime;
            yield return new WaitForSeconds(1);
        }
        float alpha = _canvasGroup.alpha;
        MainGameController.instance.popupManager.RemoveDataFromList(this._data);
        while (alpha >= 0)
        {
            alpha -= 0.1f;
            _canvasGroup.alpha = alpha;
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(this.gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _clicked = true;
    }
}
