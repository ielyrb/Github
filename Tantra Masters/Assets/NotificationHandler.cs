using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationHandler : MonoBehaviour
{
    public static NotificationHandler instance;
    [SerializeField] private GameObject itemObtainPrefab;
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private Transform itemObtainParent;
    [SerializeField] private Transform notificationParent;
    public bool isItemObtainShowing;
    public bool isNotificationShowing;

    public enum NotificationType
    { 
        Upgrade,
        Obtain,
        Craft,
        Raid,
        BossDrop        
    }

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

    public void ShowItemObtain(string playerName, Item item, int quantity)
    {
        StartCoroutine(OnShowItemObtain(playerName, item, quantity));
    }

    IEnumerator OnShowItemObtain(string playerName, Item item, int quantity)
    {
        if (isItemObtainShowing)
        {
            yield return new WaitForSeconds(0.5f);
            ShowItemObtain(playerName, item, quantity);
        }
        else
        {
            isItemObtainShowing = true;
            GameObject obj = Instantiate(itemObtainPrefab);
            ItemObtain itemObtain = obj.GetComponent<ItemObtain>();
            itemObtain.Initialize(playerName, item, quantity);
            obj.transform.SetParent(itemObtainParent, false);
        }
    }

    public void ShowNotification(string playerName, Item item, NotificationType type, string itemName, int _upgrade=0)
    {
        StartCoroutine(OnShowNotification(playerName, item, type, itemName));
    }

    IEnumerator OnShowNotification(string playerName, Item item, NotificationType _type, string itemName, int _upgrade = 0)
    {
        if (isNotificationShowing)
        {
            yield return new WaitForSeconds(0.5f);
            ShowNotification(playerName, item, _type, itemName);
        }
        else
        {
            isNotificationShowing = true;
            GameObject obj = Instantiate(notificationPrefab);
            notificationParent.gameObject.SetActive(true);
            obj.transform.SetParent(notificationParent, false);
            TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();

            string itemGrade;


            switch (item.itemGrade)
            {
                default:
                    itemGrade = "[R]";
                    break;

                case Item.ItemGrade.Epic:
                    itemGrade = "[E]";
                    break;

                case Item.ItemGrade.Legendary:
                    itemGrade = "[L]";
                    break;
            }

            switch (_type)
            {
                case NotificationType.Upgrade:
                    text.text = playerName + " Has Upgrade " + itemGrade + " " + itemName + " To +"+_upgrade;
                    break;

                case NotificationType.Craft:
                    text.text = playerName + " Has Crafted " + itemGrade + " " + itemName;
                    break;

                case NotificationType.Raid:
                    text.text = playerName + " Got " + itemGrade + " " + itemName + " From A Raid";
                    break;                                               
                                                                         
                case NotificationType.BossDrop:                          
                    text.text = playerName + " Got " + itemGrade + " " + itemName + " From A Boss";
                    break;                                               
                                                                         
                default:                                                 
                    text.text = playerName + " Got " + itemGrade + " " + itemName + " From A Monster";
                    break;
            }

            
            yield return new WaitForSeconds(13f);
            notificationParent.gameObject.SetActive(false);
            isNotificationShowing = false;
        }
    }
}
