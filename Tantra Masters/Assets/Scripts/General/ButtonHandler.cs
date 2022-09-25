using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public static ButtonHandler instance;
    public GameObject inventoryUI;
    public GameObject characterUI;

    private void Awake()
    {
        if (instance == null)
        { 
            instance = this;
            characterUI.SetActive(true);
            characterUI.SetActive(false);
        }
        else
        {
            Destroy(this);
        }
    }

    public void Inventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }

    public void Character()
    {
        characterUI.SetActive(!characterUI.activeSelf);
    }
}
