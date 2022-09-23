using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquippedItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
    [SerializeField] private Image image;
    public bool hasEquipped;
    public EquipType equipType;
    [SerializeField] private GameObject optionsUI;
    public enum EquipType
    { 
        Weapon,
        Offhand,
        Helm,
        Chest,
        Belt,
        Gloves,
        Pants,
        Boots,
        Earring1,
        Earring2,
        Bracelet1,
        Bracelet2,
        Ring1,
        Ring2,
        Charm,
        Necklace,
        Headdress,
        Eyewear,
        Mount
    }

    public void LoadItem(Item _item)
    {
        if (_item != null)
        {
            hasEquipped = true;
            item = _item;
            image.overrideSprite = InventoryHandler.instance.itemIcons[item.name];
            image.color = new Color32(255, 255, 255, 255);
        }

        PlayerData.instance.LoadItemStatModifier(_item);
    }

    private void Awake()
    {
        InventoryHandler.instance.equippedItems.Add(equipType.ToString(), this);
    }

    private void Start()
    {
        if (InventoryHandler.instance.equippedItems.Count == 19)
        {
            ItemHandler.instance.equipmentLoaded = true;
        }
    }

    public void OnClick()
    {
        optionsUI.SetActive(!optionsUI.activeSelf);
    }

    public void Clear()
    {
        hasEquipped = false;
        item = null;
        image.sprite = null;
        image.color = new Color32(255, 255, 255, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ItemTooltipHandler.instance.ShowTooltip(hasEquipped, item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipHandler.instance.HideTooltip();
    }
}
