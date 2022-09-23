using UnityEngine;
using TMPro;

public class FloatingExp : MonoBehaviour
{
    private float disappearTimer = 5f;
    [SerializeField] TextMeshProUGUI amount;
    void Update()
    {
        disappearTimer -= Time.deltaTime;

        if (disappearTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetUp(int _amount)
    {
        amount.text = _amount.ToString() + " ("+_amount+"x(100% + 0%))";
    }
}
