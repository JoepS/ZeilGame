using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SQLite4Unity3d;
using UnityEngine.UI;

public class BoatShopSceneController : MonoBehaviour {

    [SerializeField] GameObject _boatBuyPanel;
    [SerializeField] GameObject _boatBuyScrollViewContent;
    [SerializeField] Text _goldText;

    List<BoatBuyPanel> _boatBuyPanels = new List<BoatBuyPanel>();

	// Use this for initialization
	void Start () {
        _goldText.text = MainGameController.instance.localizationManager.GetLocalizedValue("gold_text") + ": " + MainGameController.instance.player.Gold;
        List<Boat> boats = GetAvaliableBoats();
        foreach(Boat b in boats)
        {
            _boatBuyPanels.Add(CreateBoatBuyPanel(b));
        }
	}

	// Update is called once per frame
	void Update () {

	}

    public void BuyBoat(Boat b)
    {
        if (MainGameController.instance.player.Gold >= b.Price)
        {
            Boat previousBoat = MainGameController.instance.databaseController.connection.Table<Boat>().Where(x => x.Active).FirstOrDefault();
            if (previousBoat != null)
            {
                previousBoat.Active = false;
                previousBoat.Save();
                Sail activeSail = previousBoat.GetSailsBought().Where(x => x.Active).First();
                if (!b.GetSailsBought().Contains(activeSail))
                {
                    activeSail.Active = false;
                    activeSail.Save();
                    Sail s = b.GetSailsBought().Where(x => x.Bought).First();
                    s.Active = true;
                    s.Save();
                }
            }
            MainGameController.instance.achievementManager.AddAchievementProperty(AchievementProperties.BoatsBought, 1);
            b.Bought = true;
            b.Active = true;
            b.Save();
            UpdateBoatBuyPanels();
            MainGameController.instance.player.GiveGold( -b.Price);
            MainGameController.instance.player.Save();
            _goldText.text = "Gold: " + MainGameController.instance.player.Gold;
        }
        else
        {
            Debug.LogError("Not enough gold to buy boat!");
        }
    }

    public void UseBoat(Boat b)
    {
        TableQuery<Boat> activeBoats = MainGameController.instance.databaseController.connection.Table<Boat>().Where(x => x.Active == true);
        if (activeBoats.Count() > 0) {
            Boat curActive = activeBoats.First();
            curActive.Active = false;
            curActive.Save();
            Sail activeSail = curActive.GetSailsBought().Where(x => x.Active).First();
            if (!b.GetSailsBought().Contains(activeSail))
            {
                activeSail.Active = false;
                activeSail.Save();
                Sail s = b.GetSailsBought().Where(x => x.Bought).First();
                s.Active = true;
                s.Save();
            }
        }
        b.Active = true;
        b.Save();
        UpdateBoatBuyPanels();
    }

    public void UpdateBoatBuyPanels()
    {
        List<Boat> boats = GetAvaliableBoats();
        for (int i = 0; i < boats.Count; i++)
        {
            _boatBuyPanels[i].SetData(boats[i], this);
        }
    }

    public List<Boat> GetAvaliableBoats()
    {
        List<int> boatIds = MainGameController.instance.player.GetCurrentLocation().GetAvaliableBoatsIds();
        List<Boat> boats = MainGameController.instance.databaseController.connection.Table<Boat>().Where(x => boatIds.Contains(x.id)).ToList();
        return boats;
    }

    public BoatBuyPanel CreateBoatBuyPanel(Boat b)
    {
        GameObject bbpGO = GameObject.Instantiate(_boatBuyPanel);
        BoatBuyPanel bbp = bbpGO.GetComponent<BoatBuyPanel>();
        bbp.SetData(b, this);
        bbpGO.transform.SetParent(_boatBuyScrollViewContent.transform);
        bbpGO.transform.localScale = Vector3.one;
        return bbp;
    }

    public void OnBackButtonClick()
    {
        MainGameController.instance.sceneController.OneSceneBack();
    }
}
