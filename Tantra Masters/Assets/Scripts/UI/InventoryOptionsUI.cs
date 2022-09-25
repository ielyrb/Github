using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryOptionsUI : MonoBehaviour
{
    public RectTransform rectTransform;
    public InventorySlot inventorySlot;
    public InventoryItem inventoryItem;

    private Vector3 origOffsetMin = new Vector2(30.5835f, -127.8785f);
    private Vector2 origOffsetMax = new Vector2(99.0165f, -22.8715f);

    private void Awake()
    {
        inventoryItem = transform.parent.GetComponent<InventoryItem>();
    }

    public void Mirror()
    {
        rectTransform.offsetMin = new Vector2(-131.8715f, -151.7215f);
        rectTransform.offsetMax = new Vector2(-36.1285f, -27.2785f);
    }

    public void Orig()
    {
        rectTransform.offsetMin = origOffsetMin;
        rectTransform.offsetMax = origOffsetMax;
    }

    public void Equip()
    {
        inventoryItem.OnClick();
        Item item = inventoryItem.item;
        if (item.canEquip)
        {
            bool state = InventoryHandler.instance.EquipItem(inventoryItem.id,item);
            if (state)
            {
                inventoryItem.Clear();
            }
        }
        else
        {
            Debug.Log("Cannot be equipped");
        }
    }

    public void Upgrade()
    {
        inventoryItem.OnClick();
        Item item = inventoryItem.item;
        ItemUpgradeHandler.instance.OnClick(inventoryItem, item);
    }

    public void Enchant()
    {
        inventoryItem.OnClick();
    }

    public void DestroyItem()
    {
        inventoryItem.OnClick();
        inventoryItem.Clear();
        PlayerData.instance.inventoryAPI.inventoryId[inventoryItem.id + 1] = null;
        string data = JsonConvert.SerializeObject(PlayerData.instance.inventoryAPI.inventoryId[inventoryItem.id+1]);
        InventoryHandler.instance.UpdateInventory(inventoryItem.id+1, data);
    }
}
