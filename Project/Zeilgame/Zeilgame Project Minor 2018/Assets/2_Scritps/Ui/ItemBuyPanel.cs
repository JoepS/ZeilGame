using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBuyPanel : MonoBehaviour {

    Item _item;
    public Item item { get { return _item; } }

    [SerializeField] Text _itemNameText;
    [SerializeField] Text _itemDescriptionText;
    [SerializeField] Button _itemBuyButton;
    [SerializeField] Text _buyButtonText;
    [SerializeField] Text _amountInInventoryText;

    public void SetData(Item i)
    {
        LocalizationManager lz = MainGameController.instance.localizationManager;

        _itemNameText.text = lz.GetLocalizedValue(i.Name);
        _itemDescriptionText.text = lz.GetLocalizedValue(i.Description);
        _buyButtonText.text = lz.GetLocalizedValue("buy_text") + "\n" + i.Price + " " + lz.GetLocalizedValue("gold_text")[0];
        _amountInInventoryText.text = i.InInventory + " " + lz.GetLocalizedValue("in_inventory_text");
        _item = i;
    }

    public Button GetItemBuyButton()
    {
        return _itemBuyButton;
    }
}
