using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoneSelectionHandler : MonoBehaviour
{
    [SerializeField] private Image upgradeStoneImage;
    [SerializeField] private Image protectionStoneImage;
    [SerializeField] private Image supportStoneImage;
    [SerializeField] private GameObject stoneSelectionUI;
    [SerializeField] private List<Image> imageList;
    private int currentType = 0;
    public void SearchStones(int id)
    {
        stoneSelectionUI.SetActive(true);
        Item.ItemType type = Item.ItemType.UpgradeStone;
        currentType = id;
        switch (id)
        {
            case 0:
                type = Item.ItemType.UpgradeStone;
                break;

            case 1:
                type = Item.ItemType.ProtectionStone;
                break;

            case 2:
                type = Item.ItemType.SupportStone;
                break;

            default:
                type = Item.ItemType.UpgradeStone;
                break;
        }

        int i = 0;
        foreach (InventoryItem inventoryItem in InventoryHandler.instance.inventoryItems)
        {
            if (i > 7) break;
            if (inventoryItem.item == null) continue;
            if (inventoryItem.item.itemType == type)
            {
                imageList[i].sprite = InventoryHandler.instance.itemIcons[inventoryItem.item.name];
                i++;
            }
        }
    }

    public void Close()
    {
        stoneSelectionUI.SetActive(false);
    }

    public void OnItemClick(int id)
    {
        Debug.Log(id);
        switch (currentType)
        {
            case 0:
                upgradeStoneImage.overrideSprite = imageList[id].sprite;
                break;

            case 1:
                protectionStoneImage.overrideSprite = imageList[id].sprite;
                break;

            case 2:
                supportStoneImage.overrideSprite = imageList[id].sprite;
                break;
        }
        Close();
    }
}
