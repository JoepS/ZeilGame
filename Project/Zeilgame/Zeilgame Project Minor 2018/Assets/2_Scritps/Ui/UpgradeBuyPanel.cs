using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeBuyPanel : MonoBehaviour
{

    [SerializeField] Text _upgradeNameText;
    [SerializeField] Text _upgradeStatsText;
    [SerializeField] Button _upgradeBuyButton;
    [SerializeField] Text _upgradeBuyText;

    Upgrade _upgradeToBuy;

    public void SetData(Upgrade u, UpgradeShopSceneController ussc)
    {
        LocalizationManager lz = MainGameController.instance.localizationManager;

        _upgradeNameText.text = u.Name;
        _upgradeStatsText.text = lz.GetLocalizedValue("speed_text") + ": \t" + u.SpeedModifier + "\n" +
                                 lz.GetLocalizedValue("height_text") + ": \t" + u.HeightModifier + "\n" +
                                 lz.GetLocalizedValue("damage_text") + ": " + u.DamageModifier + "\n" +
                                 lz.GetLocalizedValue("offline_speed_text") + ": \t" + u.OfflineSpeedModifier;
        if (!u.Bought)
        {
            _upgradeBuyText.text = lz.GetLocalizedValue("buy_text") + "\n" + u.Price + " " + lz.GetLocalizedValue("gold_text");
            _upgradeBuyButton.onClick.RemoveAllListeners();
            _upgradeBuyButton.onClick.AddListener(delegate { ussc.BuyUpgrade(u); });
        }
        else
        {
            _upgradeBuyText.text = lz.GetLocalizedValue("bought_text");
            _upgradeBuyButton.enabled = false;
        }
    }
}
