using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public int id;
    public Image image;
    public Item item;
    public bool hasItem;
    public Vector3 currentPos;

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    [SerializeField] private GameObject optionsUI;
    private Transform parentOfParent;
    public Transform currentParent;

    void Awake()
    {
        currentPos = transform.position;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GameObject.FindGameObjectWithTag("Main Canvas").GetComponent<Canvas>();
    }

    void Start()
    {
        parentOfParent = transform.parent.parent.parent;
        currentParent = transform.parent;
    }

    public void Clear()
    {
        hasItem = false;
        item = null;
        image.sprite = null;
        image.color = new Color32(255, 255, 255, 0);
    }

    public GameObject LoadNewItem(bool isFirstLoad, Item _item, Sprite _sprite)
    {
        GameObject obj = null;
        if (_item == null) return obj;
        hasItem = true;
        item = _item;
        image.overrideSprite = _sprite;
        image.color = new Color32(255, 255, 255, 255);
        obj = transform.parent.gameObject;
        InventoryId inventoryId = new InventoryId();
        inventoryId.itemName = _item.name;
        if (!isFirstLoad)
        {
            PlayerData.instance.inventoryAPI.inventoryId[id + 1] = inventoryId;
        }
        return obj;
    }

    public void OnClick()
    {
        if (!hasItem) return;
        bool state = !optionsUI.activeSelf;
        InventorySlot inventorySlot = transform.parent.GetComponent<InventorySlot>();
        
        if (state == true)
        {
            if (InventoryHandler.instance.selectedItem != this)
            {
                if (InventoryHandler.instance.selectedItem != null)
                {
                    InventoryHandler.instance.selectedItem.transform.SetParent(InventoryHandler.instance.selectedItem.currentParent);
                    InventoryHandler.instance.selectedItem.transform.GetChild(0).gameObject.SetActive(false);
                }
                InventoryHandler.instance.selectedItem = this;
            }
            InventoryHandler.instance.selectedItemOriginalPos = transform.parent;
            inventorySlot.transform.GetChild(0).GetChild(0).GetComponent<InventoryOptionsUI>().inventorySlot = inventorySlot;
            
            transform.SetParent(parentOfParent, true);

            try
            {
                if (inventorySlot.id == 5 || inventorySlot.id == 11 || inventorySlot.id == 17 || inventorySlot.id == 23)
                {
                    transform.SetParent(parentOfParent, true);
                    optionsUI.GetComponent<InventoryOptionsUI>().Mirror();
                }
                else
                {
                    optionsUI.GetComponent<InventoryOptionsUI>().Orig();
                }
            }
            catch
            { 

            }
        }
        else
        {
            if (InventoryHandler.instance.selectedItem == this)
            {
                transform.SetParent(InventoryHandler.instance.selectedItemOriginalPos, true);
            }
        }
        
        optionsUI.SetActive(state);         
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        if (optionsUI.activeSelf)
        {
            OnClick();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        transform.SetParent(parentOfParent, true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(currentParent, true);
        transform.localPosition = new Vector3(0, 0, 0);
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ItemTooltipHandler.instance.ShowTooltip(hasItem, item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipHandler.instance.HideTooltip();
    }
}
