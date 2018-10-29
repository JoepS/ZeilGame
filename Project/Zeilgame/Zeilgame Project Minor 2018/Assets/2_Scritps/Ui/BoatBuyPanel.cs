using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoatBuyPanel : MonoBehaviour {

    [SerializeField] Text _boatNameText;
    [SerializeField] Text _boatStatsText;
    [SerializeField] Button _boatBuyButton;
    [SerializeField] Text _boatBuyText;

    Boat _boatToBuy;



    public void SetData(Boat b, BoatShopSceneController bssc)
    {
        LocalizationManager lz = MainGameController.instance.localizationManager;

        _boatNameText.text = b.Name;
        _boatStatsText.text = lz.GetLocalizedValue("speed_text") + ": \t" + UnitConvertor.Convert(b.GetSpeed()) + "\n" +
                              "NGZ: \t" + b.NGZLimit + "\n" +
                              lz.GetLocalizedValue("max_damage_text") + ": \t" + b.MaxDamage + "\n" +
                              lz.GetLocalizedValue("offline_speed_text") + ": \t" + UnitConvertor.Convert(b.OfflineSpeed);
        if (!b.Bought)
        {
            _boatBuyText.text = lz.GetLocalizedValue("buy_text") + "\n" + b.Price + " " + lz.GetLocalizedValue("gold_text")[0];
            _boatBuyButton.onClick.RemoveAllListeners();
            _boatBuyButton.onClick.AddListener(delegate { bssc.BuyBoat(b); });
        }
        else if(!b.Active)
        {
            _boatBuyText.text = lz.GetLocalizedValue("use_text");
            _boatBuyButton.onClick.RemoveAllListeners();
            _boatBuyButton.onClick.AddListener(delegate { bssc.UseBoat(b); });
            _boatBuyButton.enabled = true;
        }
        else
        {
            _boatBuyText.text = lz.GetLocalizedValue("using_text");
            _boatBuyButton.enabled = false;
        }
    }
}
