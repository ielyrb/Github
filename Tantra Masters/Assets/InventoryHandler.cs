using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHandler : MonoBehaviour
{
    public static InventoryHandler instance;
    public GameObject inventoryUI;
    public GameObject equippedUI;
    public List<InventoryItem> inventoryItems;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadInventory();
        }
        else
        {
            Destroy(this);
        }
    }

    public void LoadInventory()
    {
        if (inventoryItems.Count == 0)
        {
            foreach (Transform child in inventoryUI.transform)
            {
                inventoryItems.Add(child.GetComponent<InventoryItem>());
            }
        }

        foreach (KeyValuePair<int, InventoryId> kvp in PlayerData.instance.inventoryAPI.inventoryId)
        {
            if (kvp.Value != null)
            {
                inventoryItems[kvp.Key-1].image.overrideSprite = Resources.Load<Sprite>("Images/Items/"+kvp.Value.itemName);
                inventoryItems[kvp.Key - 1].inventoryId = kvp.Value;
                inventoryItems[kvp.Key - 1].hasItem = true;
                inventoryItems[kvp.Key - 1].image.color = new Color32(255, 255, 255, 255);
            }
        }
    }
}
