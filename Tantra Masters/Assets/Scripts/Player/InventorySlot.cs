using UnityEngine;
using UnityEngine.EventSystems;
using Newtonsoft.Json;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int id;
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Transform child = transform.GetChild(0);
            Transform target = eventData.pointerDrag.transform;
            InventoryItem inventoryItem = target.GetComponent<InventoryItem>();
            target.SetParent(inventoryItem.currentParent, true);
            int targetId = target.transform.parent.GetComponent<InventorySlot>().id+1;
            //target.GetComponent<InventoryItem>().currentParent = transform;
            inventoryItem.currentParent = transform;

            child.SetParent(eventData.pointerDrag.transform.parent, false);
            child.transform.localPosition = new Vector3(0, 0, 0);
            target.transform.SetParent(transform, false);
            target.transform.localPosition = new Vector3(0, 0, 0);

            PlayerData.instance.inventoryAPI.inventoryId[id+1] = PlayerData.instance.inventoryAPI.inventoryId[targetId];
            PlayerData.instance.inventoryAPI.inventoryId[targetId] = new InventoryId();

            string s = JsonConvert.SerializeObject(PlayerData.instance.inventoryAPI.inventoryId[id + 1]);
            if (PlayerData.instance.inventoryAPI.inventoryId[id + 1] == null)
            {
                s = "NULL";
            }
            InventoryHandler.instance.UpdateInventory(id+1,s);
            s = "NULL";
            InventoryHandler.instance.UpdateInventory(targetId,s);
        }
    }
}
