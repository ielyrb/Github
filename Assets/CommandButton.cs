using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Color defaultColor;
    private Color tempColor = Color.white;

    void Awake()
    {
        defaultColor = GetComponent<Image>().color;
    }

    public void Inventory()
    {
        ButtonHandler.instance.Inventory();
    }

    public void Character()
    {
        ButtonHandler.instance.Character();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       GetComponent<Image>().color = tempColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().color = defaultColor;
    }
}
