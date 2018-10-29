using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUsePanel : MonoBehaviour {

    Item _item;
    public Item item { get { return _item; } }

    [SerializeField] Text _itemNameText;
    [SerializeField] Text _itemDescriptionText;
    [SerializeField] Button _itemUseButton;
    [SerializeField] Text _useButtonText;
    [SerializeField] Text _amountInInventoryText;

    public void SetData(Item i)
    {
        LocalizationManager lz = MainGameController.instance.localizationManager;
        _itemNameText.text = lz.GetLocalizedValue(i.Name);
        _itemDescriptionText.text = lz.GetLocalizedValue(i.Description);
        _useButtonText.text = lz.GetLocalizedValue("use_text");
        _amountInInventoryText.text = i.InInventory + " " + lz.GetLocalizedValue("in_inventory_text");
        _item = i;
    }

    public Button GetItemUseButton()
    {
        return _itemUseButton;
    }

}
