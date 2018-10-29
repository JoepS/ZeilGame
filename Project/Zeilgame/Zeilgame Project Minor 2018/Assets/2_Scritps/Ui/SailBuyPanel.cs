using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SailBuyPanel : MonoBehaviour {

    [SerializeField] Text _sailNameText;
    [SerializeField] Text _sailStatsText;
    [SerializeField] Button _sailBuyButton;
    [SerializeField] Text _sailBuyText;

    Sail _sailToBuy;

    public void SetData(Sail s, SailShopSceneController sssc)
    {
        LocalizationManager lz = MainGameController.instance.localizationManager;

        _sailNameText.text = s.Name;
        _sailStatsText.text = lz.GetLocalizedValue("max_wind_speed_text") + ": \t\t\t\t" + s.MaxWindSpeed + "\n" +
                              lz.GetLocalizedValue("speed_text") + ": \t\t\t" + s.SpeedModifier + "\n" +
                              lz.GetLocalizedValue("damage_text") + ": \t\t\t" + s.DamageModifier + "\n" +
                              lz.GetLocalizedValue("offline_speed_text") + ": \t" + s.OfflineSpeedModifier;
        if (!s.Bought)
        {
            _sailBuyText.text = lz.GetLocalizedValue("buy_text") + "\n" + s.Price + " " + lz.GetLocalizedValue("gold_text")[0];
            _sailBuyButton.onClick.RemoveAllListeners();
            _sailBuyButton.onClick.AddListener(delegate { sssc.BuySail(s); });
        }
        else if (!s.Active)
        {
            _sailBuyText.text = lz.GetLocalizedValue("use_text");
            _sailBuyButton.onClick.RemoveAllListeners();
            _sailBuyButton.onClick.AddListener(delegate { sssc.UseSail(s); });
            _sailBuyButton.enabled = true;
        }
        else
        {
            _sailBuyText.text = lz.GetLocalizedValue("using_text");
            _sailBuyButton.enabled = false;
        }
    }
}
