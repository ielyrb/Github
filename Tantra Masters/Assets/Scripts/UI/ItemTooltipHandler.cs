using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using TMPro;

public class ItemTooltipHandler : MonoBehaviour
{
    public static ItemTooltipHandler instance;
    public GameObject itemTooltip;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemGradeText;
    [SerializeField] TextMeshProUGUI itemDescText;
    [SerializeField] TextMeshProUGUI itemStatsText;
    [SerializeField] TextMeshProUGUI itemEnchantText;

    private StringBuilder sb = new StringBuilder();
    //InventoryId inventoryId;

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

    public void HideTooltip()
    {
        itemTooltip.SetActive(false);
    }

    public void ShowTooltip(bool state, Item item)
    {
        if (!state)
        {
            HideTooltip();
            return;
        }
        //inventoryId = inventoryItem.inventoryId;
        itemNameText.text = item.name;
        itemGradeText.text = item.itemGrade.ToString();
        switch (item.itemGrade)
        {
            case Item.ItemGrade.Uncommon:
                itemGradeText.color = Color.green;
                break;

            case Item.ItemGrade.Rare:
                itemGradeText.color = new Color32(0,150,255,255);
                break;

            case Item.ItemGrade.Epic:
                itemGradeText.color = Color.red;
                break;

            case Item.ItemGrade.Legendary:
                itemGradeText.color = Color.yellow;
                break;

            default:
                itemGradeText.color = Color.white;
                break;
        }
        itemDescText.text = item.itemDesc;

        sb.Length = 0;

        AddStat(item.stats.flat.hp, "HP");
        AddStat(item.stats.flat.tp, "TP");
        AddStat(item.stats.flat.patk, "PATK");
        AddStat(item.stats.flat.matk, "MATK");
        AddStat(item.stats.flat.pdef, "PDEF");
        AddStat(item.stats.flat.mdef, "MDEF");
        AddStat(item.stats.flat.hit, "Hit");
        AddStat(item.stats.flat.dodge, "Dodge");
        AddStat(item.stats.flat.crit, "Crit");
        AddStat(item.stats.flat.critEva, "Crit Eva");

        AddStat(item.stats.percent.hp, "HP", true);
        AddStat(item.stats.percent.tp, "TP", true);
        AddStat(item.stats.percent.patk, "PATK", true);
        AddStat(item.stats.percent.matk, "MATK", true);
        AddStat(item.stats.percent.pdef, "PDEF", true);
        AddStat(item.stats.percent.mdef, "MDEF", true);
        AddStat(item.stats.percent.critDamageBoost, "CD Boost", true);
        AddStat(item.stats.percent.critDamageReduc, "CD Reduc", true);
        AddStat(item.stats.percent.dropChance, "Drop Chance", true);

        itemStatsText.text = sb.ToString();

        itemTooltip.SetActive(true);
    }

    private void AddStat(float value, string statName, bool isPercent = false)
    {
        if (value != 0)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }

            if (value > 0)
            {
                sb.Append("+");
            }

            if (isPercent)
            {
                sb.Append(value);
                sb.Append("% ");
            }
            else
            {
                sb.Append(value);
                sb.Append(" ");
            }

            sb.Append(statName);
        }
    }
}
