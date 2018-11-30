using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RepairsSceneController : MonoBehaviour {

    [SerializeField] Text _damageText;

    [SerializeField] GameObject _choicePanel;
    [SerializeField] GameObject _selfFixPanel;
    [SerializeField] GameObject _payFixPanel;

    [SerializeField] Button _selfFixButton;
    [SerializeField] Button _payFixButton;

    [SerializeField] Text _goldText;

    Boat _boatToRepair;
    float _dmgPercent;

	// Use this for initialization
	void Start ()
    {
        _goldText.text = MainGameController.instance.localizationManager.GetLocalizedValue("gold_text") + ": " + MainGameController.instance.player.Gold;
        _boatToRepair = MainGameController.instance.player.GetActiveBoat();
        _dmgPercent = 0;
        if (_boatToRepair != null)
        {
            _dmgPercent = (_boatToRepair.Damage / _boatToRepair.GetMaxDamage()) * 100;
            _dmgPercent = Mathf.Round(_dmgPercent * 100) / 100;
        }
        string text = string.Format(MainGameController.instance.localizationManager.GetLocalizedValue("damage_percent_text"), _dmgPercent);
        if(Mathf.Approximately(_dmgPercent, 0))
        {
            text += "\n\n " + MainGameController.instance.localizationManager.GetLocalizedValue("no_need_to_repair_boat_text");
            _selfFixButton.gameObject.SetActive(false);
            _payFixButton.gameObject.SetActive(false);
        }
        _damageText.text = text;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnSelfFixButtonClick()
    {
        _choicePanel.SetActive(false);
        _selfFixPanel.GetComponent<RepairMiniGameController>().SetBoatToRepair(_boatToRepair);
        _selfFixPanel.SetActive(true);
    }

    public void OnPayFixButtonClick()
    {
        _choicePanel.SetActive(false);
        _payFixPanel.GetComponent<RepairShopController>().SetDamagePercent(_dmgPercent, _boatToRepair);
        _payFixPanel.SetActive(true);
    }

    public void OnBackButtonClick()
    {
        MainGameController.instance.sceneController.OneSceneBack();
    }
}
