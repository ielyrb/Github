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
    InventoryId inventoryId;

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

    public void ShowTooltip(InventoryItem item)
    {
        if (!item.hasItem)
        {
            HideTooltip();
            return;
        }
        inventoryId = item.inventoryId;
        itemNameText.text = inventoryId.itemName;
        itemGradeText.text = inventoryId.grade;
        switch (inventoryId.grade)
        {
            case "Uncommon":
                itemGradeText.color = Color.green;
                break;

            case "Rare":
                itemGradeText.color = new Color32(0,150,255,255);
                break;

            case "Epic":
                itemGradeText.color = Color.red;
                break;
        }
        itemDescText.text = inventoryId.itemDescription;

        sb.Length = 0;

        AddStat(inventoryId.stats.flat.hp, "HP");
        AddStat(inventoryId.stats.flat.tp, "TP");
        AddStat(inventoryId.stats.flat.patk, "PATK");
        AddStat(inventoryId.stats.flat.matk, "MATK");
        AddStat(inventoryId.stats.flat.pdef, "PDEF");
        AddStat(inventoryId.stats.flat.mdef, "MDEF");
        AddStat(inventoryId.stats.flat.hit, "Hit");
        AddStat(inventoryId.stats.flat.dodge, "Dodge");
        AddStat(inventoryId.stats.flat.crit, "Crit");
        AddStat(inventoryId.stats.flat.critEva, "Crit Eva");

        AddStat(inventoryId.stats.percent.hp, "HP", true);
        AddStat(inventoryId.stats.percent.tp, "TP", true);
        AddStat(inventoryId.stats.percent.patk, "PATK", true);
        AddStat(inventoryId.stats.percent.matk, "MATK", true);
        AddStat(inventoryId.stats.percent.pdef, "PDEF", true);
        AddStat(inventoryId.stats.percent.mdef, "MDEF", true);
        AddStat(inventoryId.stats.percent.critDmgBoost, "CD Boost", true);
        AddStat(inventoryId.stats.percent.critDmgReduc, "CD Reduc", true);
        AddStat(inventoryId.stats.percent.dropChance, "Drop Chance", true);

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
