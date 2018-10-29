using SQLite4Unity3d;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SailShopSceneController : MonoBehaviour {

    [SerializeField] GameObject _sailBuyPanel;
    [SerializeField] GameObject _sailBuyScrollViewContent;
    [SerializeField] Text _goldText;

    List<SailBuyPanel> _sailBuyPanels = new List<SailBuyPanel>();

    // Use this for initialization
    void Start () {
        _goldText.text = MainGameController.instance.localizationManager.GetLocalizedValue("gold_text") + ": " + MainGameController.instance.player.Gold;
        List<Sail> sails = MainGameController.instance.databaseController.connection.Table<Sail>().ToList();
        foreach (Sail b in sails)
        {
            Debug.Log(b.id + " / " + b.Name + " / " + b.Price + " / " + b.Bought);
            _sailBuyPanels.Add(CreateSailBuyPanel(b));
        }
    }

    public void BuySail(Sail s)
    {
        if (MainGameController.instance.player.Gold >= s.Price)
        {
            Sail previousSail = MainGameController.instance.databaseController.connection.Table<Sail>().Where(x => x.Active).FirstOrDefault();
            if(previousSail != null)
            {
                previousSail.Active = false;
                previousSail.Save();
            }

            Debug.Log("Buy sail: " + s);
            s.Bought = true;
            s.Active = true;
            s.Save();
            UpdateSailBuyPanels();
            MainGameController.instance.player.Gold -= s.Price;
            MainGameController.instance.player.Save();
            _goldText.text = MainGameController.instance.localizationManager.GetLocalizedValue("gold_text") + ": " + MainGameController.instance.player.Gold;
        }
        else
        {
            Debug.Log("Not enough gold to buy sail!");
        }
    }

    public void UseSail(Sail s)
    {
        Debug.Log("Use sail: " + s);
        TableQuery<Sail> activeSails = MainGameController.instance.databaseController.connection.Table<Sail>().Where(x => x.Active == true);
        if (activeSails.Count() > 0)
        {
            Sail curActive = activeSails.First();
            curActive.Active = false;
            curActive.Save();
        }
        s.Active = true;
        s.Save();
        UpdateSailBuyPanels();
    }

    public void UpdateSailBuyPanels()
    {
        List<Sail> sails = MainGameController.instance.databaseController.connection.Table<Sail>().ToList();
        for (int i = 0; i < sails.Count; i++)
        {
            _sailBuyPanels[i].SetData(sails[i], this);
        }
    }

    public SailBuyPanel CreateSailBuyPanel(Sail b)
    {
        GameObject sbpGO = GameObject.Instantiate(_sailBuyPanel);
        SailBuyPanel sbp = sbpGO.GetComponent<SailBuyPanel>();
        sbp.SetData(b, this);
        sbpGO.transform.SetParent(_sailBuyScrollViewContent.transform);
        sbpGO.transform.localScale = Vector3.one;
        return sbp;
    }

    public void OnBackButtonClick()
    {
        MainGameController.instance.sceneController.OneSceneBack();
    }
}
