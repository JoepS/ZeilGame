using SQLite4Unity3d;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShopSceneController : MonoBehaviour
{

    [SerializeField] GameObject _upgradeBuyPanel;
    [SerializeField] GameObject _upgradeBuyScrollViewContent;
    [SerializeField] Text _goldText;

    List<UpgradeBuyPanel> _upgradeBuyPanels = new List<UpgradeBuyPanel>();

    // Use this for initialization
    void Start()
    {
        _goldText.text = MainGameController.instance.localizationManager.GetLocalizedValue("gold_text") + ": " + MainGameController.instance.player.Gold;
        List<Upgrade> upgrades = GetUpgradesListCurBoat();
        foreach (Upgrade u in upgrades)
        {
            _upgradeBuyPanels.Add(CreateUpgradeBuyPanel(u));
        }
    }

    public List<Upgrade> GetUpgradesListCurBoat()
    {
        List<int> ids = new List<int>();
        if (MainGameController.instance.player.GetActiveBoat() != null)
        {
            ids = MainGameController.instance.player.GetActiveBoat().GetUpgradesId();
        }
        return MainGameController.instance.databaseController.connection.Table<Upgrade>().Where(x => ids.Contains(x.id)).ToList();
    }

    public void BuyUpgrade(Upgrade u)
    {
        if (MainGameController.instance.player.Gold >= u.Price && !u.Bought)
        {
            u.Bought = true;
            u.Save();
            UpdateUpgradeBuyPanels();
            MainGameController.instance.player.GiveGold(-u.Price);
            MainGameController.instance.player.Save();
            _goldText.text = MainGameController.instance.localizationManager.GetLocalizedValue("gold_text") + ": " + MainGameController.instance.player.Gold;
        }
        else
        {
            Debug.LogError("Not enough gold to buy upgrade!");
        }
    }

    public void UpdateUpgradeBuyPanels()
    {
        List<Upgrade> upgrades = GetUpgradesListCurBoat();
        for (int i = 0; i < upgrades.Count; i++)
        {
            _upgradeBuyPanels[i].SetData(upgrades[i], this);
        }
    }

    public UpgradeBuyPanel CreateUpgradeBuyPanel(Upgrade u)
    {
        GameObject ubpGO = GameObject.Instantiate(_upgradeBuyPanel);
        UpgradeBuyPanel ubp = ubpGO.GetComponent<UpgradeBuyPanel>();
        ubp.SetData(u, this);
        ubpGO.transform.SetParent(_upgradeBuyScrollViewContent.transform);
        ubpGO.transform.localScale = Vector3.one;
        return ubp;
    }

    public void OnBackButtonClick()
    {
        MainGameController.instance.sceneController.OneSceneBack();
    }
}
