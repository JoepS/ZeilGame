using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopup : MonoBehaviour {

    [SerializeField] Text _itemPopupText;
    bool _moveItemPopup = false;
    Vector2 _startPos;

    public void SetText(string text)
    {
        _startPos = this.transform.localPosition;
        _itemPopupText.text = text;
        _moveItemPopup = true;
    }

	// Update is called once per frame
	void Update () {
        if (_moveItemPopup)
        {
            Vector3 pos = this.transform.localPosition;
            pos.y += 10;
            pos.x = pos.x + Mathf.Cos(pos.y / 100) * 10;
            this.transform.localPosition = pos;
            float a = 1 - (Vector2.Distance(this.transform.localPosition, _startPos) / 1000);
            this.GetComponent<CanvasGroup>().alpha = a;
            if(a <= 0)
            {
                Destroy(this.gameObject);
            }
        }
	}
}
