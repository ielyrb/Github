using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemObtain : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private float showDuration = 5f;

    public void Initialize(string playerName, Item item, int quantity)
    {
        string itemGrade;
        switch (item.itemGrade)
        {
            case (Item.ItemGrade)1:
                itemGrade = "[UC]";
                itemName.color = Color.green;
                break;

            case (Item.ItemGrade)2:
                itemGrade = "[R]";
                itemName.color = Color.blue;
                break;

            case (Item.ItemGrade)3:
                itemGrade = "[E]";
                itemName.color = Color.red;
                break;

            case (Item.ItemGrade)4:
                itemGrade = "[L]";
                itemName.color = Color.yellow;
                break;

            default:
                itemGrade = "[C]";
                itemName.color = Color.white;
                break;
        }

        Sprite sprite = InventoryHandler.instance.itemIcons[item.name];
        image.sprite = sprite;
        itemName.text = itemGrade +" "+ item.name + " x"+quantity;
        Destroy(gameObject, showDuration);
    }

    private void OnDestroy()
    {
        NotificationHandler.instance.isItemObtainShowing = false;
    }
}
