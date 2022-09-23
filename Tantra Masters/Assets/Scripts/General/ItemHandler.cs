using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    public static ItemHandler instance;
    public ItemList itemDb;
    public bool equipmentLoaded;
    public bool playerLoaded;
    private bool isLoaded;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Item[] items = Resources.LoadAll<Item>("Items");

            foreach (Item item in items)
            {
                if (itemDb.itemDictionary.ContainsKey(item.name))
                {
                    continue;
                }
                else
                {
                    itemDb.itemDictionary.Add(item.name, item);
                }
            }
        }
    }

    private void Update()
    {
        if (isLoaded) return;
        if (equipmentLoaded && playerLoaded)
        {
            LoadItems();
        }
    }

    public void LoadItems()
    {
        foreach (KeyValuePair<string, EquipmentData> kvp in PlayerData.instance.equippedAPI.equipmentdata)
        {
            if (kvp.Value.itemName != null)
            {
                Item item = itemDb.itemDictionary[kvp.Value.itemName];
                LoadEquippedItems(UppercaseFirst(kvp.Key), item);
            }
        }
        isLoaded = true;
    }

    public void LoadEquippedItems(string type, Item item)
    {
        foreach (KeyValuePair<string, EquippedItem> kvp in InventoryHandler.instance.equippedItems)
        {
            if (kvp.Value.equipType.ToString() == type)
            {
                if (!kvp.Value.hasEquipped)
                {
                    kvp.Value.LoadItem(item);
                    break;
                }
            }
        }
        InventoryHandler.instance.inventoryParent.SetActive(false);
    }

    string UppercaseFirst(string s)
    {
        // Check for empty string.
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        // Return char and concat substring.
        return char.ToUpper(s[0]) + s.Substring(1);
    }
}
