using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class ItemDropManager : MonoBehaviour
{
    public static ItemDropManager instance;
    public List<ItemDropTable> dropTable = new List<ItemDropTable>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public Item InitializeLoot(string playerName, int _id)
    {
        Item item = GetLoot(_id);
        if (item != null)
        {
            foreach (InventoryItem inventory in InventoryHandler.instance.inventoryItems)
            {
                if (!inventory.hasItem)
                {
                    Sprite sprite = InventoryHandler.instance.itemIcons[item.name];
                    GameObject obj = inventory.LoadNewItem(false, item, sprite);
                    if (obj != null)
                    {
                        NotificationHandler.instance.ShowItemObtain(playerName, item, 1);
                        int id = obj.GetComponent<InventorySlot>().id;
                        string data = JsonConvert.SerializeObject(PlayerData.instance.inventoryAPI.inventoryId[id+1]);;
                        InventoryHandler.instance.UpdateInventory(id+1, data);
                    }
                    break;
                }
            }
        }
        return item;
    }

    private Item GetLoot(int _id)
    {
        float maxroll = 0;
        Item reward = null;

        foreach (DropItem item in dropTable[_id].drops)
        {
            maxroll += item.dropChance;
        }

        float roll = UnityEngine.Random.Range(0, maxroll);

        foreach (DropItem drop in dropTable[_id].drops)
        {
            roll -= drop.dropChance;

            if (roll <= 0)
            {
                foreach (KeyValuePair<string, Item> kvp in ItemHandler.instance.itemDb.itemDictionary)
                {
                    if (kvp.Key == drop.item.name)
                    {
                        reward = kvp.Value;
                        break;
                    }
                }
                break;
            }
        }
        return reward;
    }

    public Tuple<bool,int> GetItem(Item item)
    {
        bool result = false;
        int id = 0;
        foreach (InventoryItem inventory in InventoryHandler.instance.inventoryItems)
        {
            if (!inventory.hasItem)
            {
                Sprite sprite = InventoryHandler.instance.itemIcons[item.name];
                GameObject obj = inventory.LoadNewItem(false, item, sprite);
                if (obj != null)
                {
                    id = obj.GetComponent<InventorySlot>().id;
                    string data = JsonConvert.SerializeObject(PlayerData.instance.inventoryAPI.inventoryId[id + 1]); ;
                    InventoryHandler.instance.UpdateInventory(id + 1, data);
                }
                break;
            }
        }
        return new Tuple<bool, int>(result,id+1);
        //return result;
    }
}
