using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RandomItemCrate : MonoBehaviour, IPointerDownHandler{

    List<GameObject> _seaBlocks;
    public List<GameObject> SeaBlocks { set { _seaBlocks = value; _seaBlocksIndex = value.Count-1; StartCoroutine(MoveCrate()); } }
    int _seaBlocksIndex = 0;

    int _randomGoldAmount;
    public int RandomGoldAmount { set { _randomGoldAmount = value; } }
    Item _randomItem;
    public Item RandomItem { set { _randomItem = value; } }


    GameObject _itemPopupPrefab;
    public GameObject ItemPopupPrefab { set { _itemPopupPrefab = value; } }

    // Update is called once per frame
    void Update () {
        
    }

    IEnumerator MoveCrate()
    {
        this.transform.position = _seaBlocks[_seaBlocksIndex].transform.position;
        float heightOffset = Random.Range(-500, 0);
        while (_seaBlocksIndex >= 0)
        {
            GameObject seaBlockToFollow = _seaBlocks[_seaBlocksIndex];

            Vector2 size = seaBlockToFollow.GetComponent<RectTransform>().sizeDelta;
            float height = (size.y / 2);// - (Screen.height / 2);// _seaBlockToFollow.GetComponent<RectTransform>().position.y +

            Vector3 pos = seaBlockToFollow.transform.localPosition;
            pos.z = 1;
            pos.y = height + heightOffset;
            this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, pos, Time.deltaTime * 50);
            if (Mathf.Abs(this.transform.localPosition.x - pos.x) < 0.1f) 
                _seaBlocksIndex -= 1;
            yield return new WaitForEndOfFrame();
        }
        Destroy(this.gameObject);
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        
        GameObject go = GameObject.Instantiate(_itemPopupPrefab);
        go.transform.SetParent(this.transform.parent.parent);
        go.transform.localPosition = this.transform.localPosition;
        go.transform.localPosition += new Vector3(0, 0, -1);
        go.transform.localScale = Vector3.one;
        MainGameController.instance.achievementManager.AddAchievementProperty(AchievementProperties.CratesOpened, 1);
        if (_randomItem == null)
        {
            MainGameController.instance.player.GiveGold(_randomGoldAmount);
            go.GetComponent<ItemPopup>().SetText(_randomGoldAmount + "\n" + MainGameController.instance.localizationManager.GetLocalizedValue("gold_text"));
        }
        else
        {
            _randomItem.InInventory++;
            _randomItem.Save();
            go.GetComponent<ItemPopup>().SetText(MainGameController.instance.localizationManager.GetLocalizedValue(_randomItem.Name));
        }
        Destroy(this.gameObject);
    }
}
