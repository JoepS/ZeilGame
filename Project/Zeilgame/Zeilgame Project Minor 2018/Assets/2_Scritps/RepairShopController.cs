using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RepairShopController : MonoBehaviour {

    [SerializeField] Text _damageText;

    [SerializeField] Button _10Button;
    float _cost10;
    [SerializeField] Button _25Button;
    float _cost25;
    [SerializeField] Button _50Button;
    float _cost50;
    [SerializeField] Button _75Button;
    float _cost75;
    [SerializeField] Button _100Button;
    float _cost100;

    [SerializeField] Text _goldText;

    Boat _boatToRepair;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetDamagePercent(float percent, Boat boatToRepair)
    {
        _boatToRepair = boatToRepair;
        SetDamagePercent(percent);
        LocalizationManager lz = MainGameController.instance.localizationManager;
        string fixpercentforgoldtext = lz.GetLocalizedValue("fix_percent_for_gold_text");
        _cost10 = _boatToRepair.Price * 0.1f;
        _10Button.GetComponentInChildren<Text>().text = string.Format(fixpercentforgoldtext, 10, _cost10);
        _cost25 = _boatToRepair.Price * 0.25f;
        _25Button.GetComponentInChildren<Text>().text = string.Format(fixpercentforgoldtext, 25, _cost25);
        _cost50 = _boatToRepair.Price * 0.5f;
        _50Button.GetComponentInChildren<Text>().text = string.Format(fixpercentforgoldtext, 50, _cost50);
        _cost75 = _boatToRepair.Price * 0.75f;
        _75Button.GetComponentInChildren<Text>().text = string.Format(fixpercentforgoldtext, 75, _cost75);
        _cost100 = _boatToRepair.Price * 1f;
        _100Button.GetComponentInChildren<Text>().text = string.Format(fixpercentforgoldtext, 100, _cost100);
    }

    public void SetDamagePercent(float percent)
    {
        if (Mathf.Approximately(percent, 0))
        {
            percent = 0;
            _damageText.text = string.Format(MainGameController.instance.localizationManager.GetLocalizedValue("damage_percent_text"), percent) + "\n" +
                               "" + MainGameController.instance.localizationManager.GetLocalizedValue("no_need_to_repair_boat_text");
        }
        else
        {
            _damageText.text = string.Format(MainGameController.instance.localizationManager.GetLocalizedValue("damage_percent_text"), percent) + "\n(" + _boatToRepair.Damage + " / " + _boatToRepair.GetMaxDamage() + ")"; 
            _goldText.text = MainGameController.instance.localizationManager.GetLocalizedValue("gold_text") + ": " + MainGameController.instance.player.Gold;
        }
        if(Mathf.Approximately(percent, 0))
        {
            _10Button.gameObject.SetActive(false);
            _25Button.gameObject.SetActive(false);
            _50Button.gameObject.SetActive(false);
            _75Button.gameObject.SetActive(false);
            _100Button.gameObject.SetActive(false);
        }
        else if (percent < 10)
        {
            _10Button.gameObject.SetActive(true);
            _25Button.gameObject.SetActive(false);
            _50Button.gameObject.SetActive(false);
            _75Button.gameObject.SetActive(false);
            _100Button.gameObject.SetActive(false);
        }
        else if (percent < 25)
        {

            _10Button.gameObject.SetActive(true);
            _25Button.gameObject.SetActive(true);
            _50Button.gameObject.SetActive(false);
            _75Button.gameObject.SetActive(false);
            _100Button.gameObject.SetActive(true);
        }
        else if (percent < 50)
        {

            _10Button.gameObject.SetActive(true);
            _25Button.gameObject.SetActive(true);
            _50Button.gameObject.SetActive(true);
            _75Button.gameObject.SetActive(false);
            _100Button.gameObject.SetActive(true);
        }
        else
        {

            _10Button.gameObject.SetActive(true);
            _25Button.gameObject.SetActive(true);
            _50Button.gameObject.SetActive(true);
            _75Button.gameObject.SetActive(true);
            _100Button.gameObject.SetActive(true);
        }
    }

    public void RepairDamage(float amountPercent)
    {
        int cost = Mathf.RoundToInt(_boatToRepair.Price * amountPercent);

        if (MainGameController.instance.player.Gold >= cost)
        {
            MainGameController.instance.player.GiveGold(-cost);
            _boatToRepair.Damage -= _boatToRepair.GetMaxDamage() * amountPercent;
            if (_boatToRepair.Damage <= 0)
                _boatToRepair.Damage = 0;
            _boatToRepair.Save();
            float newPercent = (_boatToRepair.Damage / _boatToRepair.GetMaxDamage()) * 100;
            SetDamagePercent(newPercent);
        }
        else
        {
            Debug.LogError("Not enough money: " + cost);
        }
    }
}
