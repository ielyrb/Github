using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class InventoryHandler : MonoBehaviour
{
    public static InventoryHandler instance;
    public GameObject inventoryParent;
    public GameObject inventoryUI;
    public GameObject equippedUI;
    public GameObject inventoryItemPrefab;
    public List<InventoryItem> inventoryItems;
    public List<InventorySlot> inventorySlots;
    public InventoryItem selectedItem;
    public Transform selectedItemPos;
    public Transform selectedItemOriginalPos;
    public Dictionary<string, EquippedItem> equippedItems = new();
    public Dictionary<string, Sprite> itemIcons = new();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Items");

            foreach (Sprite spr in sprites)
            {
                itemIcons.Add(spr.name, spr);
            }

            inventoryParent.SetActive(true);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        LoadInventory();

        foreach (KeyValuePair<string, Sprite> kvp in itemIcons)
        {
            ItemHandler.instance.itemDb.AddNewItem(kvp.Key, "");
        }
    }

    public bool EquipItem(int id, Item item)
    {
        bool result = false;
        string _equipType = item.itemType.ToString();
        switch (item.itemType)
        {
            case Item.ItemType.Ring:
                if (!equippedItems["Ring1"].hasEquipped)
                {
                    _equipType = EquippedItem.EquipType.Ring1.ToString();
                    break;
                }

                if (!equippedItems["Ring2"].hasEquipped)
                {
                    _equipType = EquippedItem.EquipType.Ring2.ToString();
                    break;
                }
                _equipType = EquippedItem.EquipType.Ring1.ToString();
                break;

            case Item.ItemType.Bracelet:
                if (!equippedItems["Bracelet1"].hasEquipped)
                {
                    _equipType = EquippedItem.EquipType.Bracelet1.ToString();
                    break;
                }

                if (!equippedItems["Bracelet2"].hasEquipped)
                {
                    _equipType = EquippedItem.EquipType.Bracelet2.ToString();
                    break;
                }
                _equipType = EquippedItem.EquipType.Bracelet1.ToString();
                break;

            case Item.ItemType.Earring:
                if (!equippedItems["Earring1"].hasEquipped)
                {
                    _equipType = EquippedItem.EquipType.Earring1.ToString();
                    break;
                }

                if (!equippedItems["Earring2"].hasEquipped)
                {
                    _equipType = EquippedItem.EquipType.Earring2.ToString();
                    break;
                }
                _equipType = EquippedItem.EquipType.Earring1.ToString();
                break;
        }

        EquippedItem.EquipType equipType = (EquippedItem.EquipType)Enum.Parse(typeof(EquippedItem.EquipType),_equipType);

        EquippedItem equipped = equippedItems[equipType.ToString()];
        if (!equipped.hasEquipped)
        {
            equipped.hasEquipped = true;
            equipped.LoadItem(item);
            result = true;
            EquipmentData equipmentData = new();
            equipmentData.itemName = item.name;
            //PlayerData.instance.equippedAPI.equipmentdata.Add(equipType.ToString(), equipmentData);
            UpdateEquipped(equipped.equipType.ToString(), item.name);
            UpdateInventory(id + 1, "NULL");
        }
        return result;
    }

    public bool UnequipItem(string equipType, Item item)
    {
        bool result = false;
        UpdateEquipped(equipType.ToString(), "NULL");
        Tuple<bool, int> _result = ItemDropManager.instance.GetItem(item);
        string data = JsonConvert.SerializeObject(PlayerData.instance.inventoryAPI.inventoryId[_result.Item2]);
        UpdateInventory(_result.Item2, data);
        return result;
    }

    void LoadInventory()
    {
        if (inventoryItems.Count == 0)
        {
            foreach (Transform child in inventoryUI.transform)
            {
                inventorySlots.Add(child.GetComponent<InventorySlot>());
                var newItem = Instantiate(inventoryItemPrefab, inventoryItemPrefab.transform.position, Quaternion.identity);
                newItem.transform.SetParent(child, false);
                inventoryItems.Add(newItem.GetComponent<InventoryItem>());
            }
        }

        int i = 0;

        foreach (KeyValuePair<int, InventoryId> kvp in PlayerData.instance.inventoryAPI.inventoryId)
        {
            if (kvp.Value != null)
            {
                Sprite sprite = itemIcons[kvp.Value.itemName];
                if (kvp.Value.itemName != null)
                {
                    inventoryItems[kvp.Key - 1].LoadNewItem(true,ItemHandler.instance.itemDb.itemDictionary[kvp.Value.itemName], sprite);
                }
            }
            inventoryItems[kvp.Key - 1].id = i;
            inventorySlots[kvp.Key - 1].GetComponent<InventorySlot>().id = i;
            i++;
        }
    }

    public void UpdateInventory(int id, string data)
    {
        string uri = Globals.updateinventory + "charname=" + PlayerData.instance.userDataAPI.UserData.userId + "&columnName=item" + id + "&data=" + data;
        StartCoroutine(OnUpdateInventory(uri));
    }

    IEnumerator OnUpdateInventory(string uri)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
        }
    }

    public void UpdateEquipped(string columnName, string data)
    {
        string uri = Globals.updateequipped + "charname=" + PlayerData.instance.userDataAPI.UserData.userId + "&columnName=" + columnName + "&data=" + data;
        StartCoroutine(OnUpdateEquipped(uri));
    }

    IEnumerator OnUpdateEquipped(string uri)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
        }
    }
}
