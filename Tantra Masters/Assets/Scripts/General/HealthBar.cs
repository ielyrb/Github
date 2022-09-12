using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI healthText;
    private int maxHealth;

    public void SetMaxHealth(int amount)
    {
        slider.maxValue = amount;
        maxHealth = amount;
    }

    public void SetHealth(int amount)
    {
        slider.value = amount;
        healthText.text = amount + " / " + maxHealth;
    }
}
