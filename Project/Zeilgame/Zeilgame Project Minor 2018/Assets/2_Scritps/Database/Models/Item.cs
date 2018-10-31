using UnityEngine;
using SQLite4Unity3d;

[Table("Item")]
public class Item : Model {
    public string Name { get; set; }
    public int Price { get; set; }
    public string Description { get; set; }
    public int InInventory { get; set; }

    public override string ToString()
    {
        return id + " / " + Name + " / " + Price + " / " + Description + " / " + InInventory;
    }

    public override void Reset()
    {
        this.InInventory = 0;
        this.Save();
    }

    public void UseItem()
    {
        switch (id)
        {
            case 1:
                //Repair Kit
                MainGameController.instance.player.GetActiveBoat().Damage -= MainGameController.instance.player.GetActiveBoat().GetMaxDamage() * 0.1f;
                if (MainGameController.instance.player.GetActiveBoat().Damage < 0)
                    MainGameController.instance.player.GetActiveBoat().Damage = 0;
                MainGameController.instance.player.GetActiveBoat().Save();
                break;
            default:
                Debug.LogError("Unknown item to use: " + this);
                break;
        }
    }

    public override void Copy(Model m)
    {
        Item i = (Item)m;
        this.Name = i.Name;
        this.Price = i.Price;
        this.Description = i.Description;
    }
}
