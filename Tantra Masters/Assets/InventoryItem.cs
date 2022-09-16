using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public InventoryId inventoryId;
    public bool hasItem;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ItemTooltipHandler.instance.ShowTooltip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipHandler.instance.HideTooltip();
    }
}
