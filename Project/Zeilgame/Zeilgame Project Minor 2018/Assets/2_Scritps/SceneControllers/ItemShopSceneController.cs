using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ItemShopSceneController : MonoBehaviour {

    [SerializeField] Text _goldText;
    [SerializeField] GameObject _itemBuyPanel;
    [SerializeField] GameObject _itemBuyViewContent;

    List<ItemBuyPanel> _itemBuyPanels;

    // Use this for initialization
    void Start() {
        _itemBuyPanels = new List<ItemBuyPanel>();
        CreateItemBuyPanels();
        _goldText.text = MainGameController.instance.localizationManager.GetLocalizedValue("gold_text") + ": " + MainGameController.instance.player.Gold;
    }

    // Update is called once per frame
    void Update() {

    }

    public void CreateItemBuyPanels()
    {
        List<Item> items = MainGameController.instance.databaseController.connection.Table<Item>().ToList();
        foreach(Item i in items)
        {
            GameObject go = GameObject.Instantiate(_itemBuyPanel);
            go.GetComponent<ItemBuyPanel>().SetData(i);
            go.transform.SetParent(_itemBuyViewContent.transform);
            go.transform.localScale = Vector3.one;
            _itemBuyPanels.Add(go.GetComponent<ItemBuyPanel>());
            go.GetComponent<ItemBuyPanel>().GetItemBuyButton().onClick.AddListener(delegate { OnItemBuyClick(i); });
        }
    }

    public void OnItemBuyClick(Item i)
    {
        if(MainGameController.instance.player.Gold >= i.Price)
        {
            i.InInventory++;
            i.Save();
            MainGameController.instance.player.GiveGold(-i.Price);
            MainGameController.instance.player.Save();
            _goldText.text = MainGameController.instance.player.Gold + "G";
            _itemBuyPanels.Where(x => x.item.id == i.id).First().SetData(i);
        }
    }

    public void OnBackButtonClick()
    {
        MainGameController.instance.sceneController.OneSceneBack();
    }
}
