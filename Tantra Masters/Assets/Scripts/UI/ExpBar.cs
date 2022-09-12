using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpBar : MonoBehaviour
{
    public static ExpBar instance;
    public Slider slider;
    public TextMeshProUGUI amountText;
    int prevAmount;
    int maxAmount;

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

    public void SetMaxExp(int amount)
    {
        slider.maxValue = amount;
        maxAmount = amount;
    }

    public void SetExp(int amount)
    {
        slider.value = amount;
        float percentage = (float)(amount * 100 / maxAmount);
        amountText.text = percentage.ToString("F2");
    }
}
