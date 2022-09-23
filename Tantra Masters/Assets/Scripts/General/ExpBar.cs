using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpBar : MonoBehaviour
{
    public static ExpBar instance;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private GameObject floatingExpPrefab;
    [SerializeField] private Transform expLogParent;
    
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
        double percentage = (amount * 100 / maxAmount);
        amountText.text = percentage.ToString("F3");
    }

    public void ShowFloatingExp(int amount)
    {
        GameObject obj = Instantiate(floatingExpPrefab, floatingExpPrefab.transform.position, Quaternion.identity);
        obj.GetComponent<FloatingExp>().SetUp(amount);
        if (expLogParent.transform.childCount >= 5)
        {
            Destroy(expLogParent.transform.GetChild(0).gameObject);
        }
        obj.transform.SetParent(expLogParent, false);
        obj.transform.localPosition = new Vector3(0, 0, 0);
    }
}
