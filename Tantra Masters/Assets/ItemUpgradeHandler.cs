using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemUpgradeHandler : MonoBehaviour
{
    public static ItemUpgradeHandler instance;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private TextMeshProUGUI maxUpgradeText;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI ugpradeLevelText;

    [SerializeField] private GameObject stoneSelectionUI;
    [SerializeField] private StoneSelectionHandler stoneSelectionHandler;
    private Item item;

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

    public void OnClick(InventoryItem inventoryItem, Item item)
    {
        itemNameText.text = item.name;
        itemIcon.sprite = inventoryItem.image.overrideSprite;
        switch (item.itemGrade)
        {
            case Item.ItemGrade.Uncommon:
                maxUpgradeText.text = "Max Upgrade: 3";
                break;

            case Item.ItemGrade.Rare:
                maxUpgradeText.text = "Max Upgrade: 5";
                break;

            case Item.ItemGrade.Epic:
                maxUpgradeText.text = "Max Upgrade: 7";
                break;

            case Item.ItemGrade.Legendary:
                maxUpgradeText.text = "Max Upgrade: 10";
                break;

            default:
                maxUpgradeText.text = "Max Upgrade: 1";
                break;
        }
        upgradeUI.SetActive(true);
    }

    public void UpgradeStone()
    {
        Debug.Log("Upgrade Stone clicked");
        //stoneSelectionHandler.gameObject.SetActive(true);
        stoneSelectionHandler.SearchStones(0);
    }

    public void ProtectionStone()
    {
        Debug.Log("Protection Stone clicked");
        stoneSelectionHandler.SearchStones(1);
    }

    public void SupportStone()
    {
        Debug.Log("Support Stone clicked");
        stoneSelectionHandler.SearchStones(2);
    }

    public void Upgrade()
    {
        Debug.Log("Upgrade Clicked");
    }

    public void Cancel()
    {
        Debug.Log("Cancel Clicked");
        upgradeUI.SetActive(false);
    }
}
