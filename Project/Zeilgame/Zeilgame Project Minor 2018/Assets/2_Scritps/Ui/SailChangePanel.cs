using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SailChangePanel : MonoBehaviour {

    [SerializeField] Text _sailNameText;
    [SerializeField] Text _sailStatsText;
    [SerializeField] Button _sailEquipButton;
    [SerializeField] Text _sailEquipText;

    public void SetData(Sail s)
    {
        LocalizationManager lz = MainGameController.instance.localizationManager;
        _sailNameText.text = s.Name;
        _sailStatsText.text = lz.GetLocalizedValue("max_wind_speed_text") + ": " + s.MaxWindSpeed;// + "\nSpeed: \t\t\t" + s.SpeedModifier + "\nDamage: \t\t\t" + s.DamageModifier + "\nOffline Speed: \t" + s.OfflineSpeedModifier;
        _sailEquipText.text = lz.GetLocalizedValue("equip_text");
        if (s.Active)
            _sailEquipText.text = lz.GetLocalizedValue("active_text");
    }

    public Button GetButton()
    {
        return _sailEquipButton;
    }

    public void OnButtonClick()
    {
        _sailEquipText.text = MainGameController.instance.localizationManager.GetLocalizedValue("active_text");
    }
}
