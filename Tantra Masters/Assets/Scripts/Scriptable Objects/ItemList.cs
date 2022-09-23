using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemList", menuName = "Item List", order = 0)]
public class ItemList : ScriptableObject
{
    [NonReorderable]
    public List<ItemListData> itemList;
    public Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();

    public void AddNewItem(string _itemName, string _itemDesc)
    {
        bool hasItem = false;
        int _itemId = itemList.Count;

        foreach (ItemListData data in itemList)
        {
            if (data.itemName == _itemName)
            {
                hasItem = true;
                break;
            }
        }

        if (!hasItem)
        {
            itemList.Add(new ItemListData { itemId = _itemId, itemName = _itemName, itemDesc = _itemDesc });
        }
    }
}

[System.Serializable]
public class ItemListData
{
    public int itemId;
    public string itemName;
    public string itemDesc;
}
