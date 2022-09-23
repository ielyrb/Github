using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippedOptionsUI : MonoBehaviour
{
    private EquippedItem equippedItem;
    private void Awake()
    {
        equippedItem = transform.parent.GetComponent<EquippedItem>();
    }

    public void UnEquip()
    {
        equippedItem.OnClick();
        InventoryHandler.instance.UnequipItem(equippedItem.equipType.ToString(),equippedItem.item);
        equippedItem.Clear();
        //PlayerData.instance.ReloadStats();        
    }

    public void Upgrade()
    {
        equippedItem.OnClick();
    }

    public void Enchant()
    {
        equippedItem.OnClick();
    }
}
