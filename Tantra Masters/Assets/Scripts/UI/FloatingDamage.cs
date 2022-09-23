using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingDamage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI damageText;
    private float moveYSpeed = 0.2f;
    private float disappearTimer = 2f;
    private float disappearSpeed = 3f;
    private Color textColor;

    private void Start()
    {
        textColor = damageText.color;
    }

    private void Update()
    {
        transform.position += new Vector3(0,moveYSpeed) * Time.deltaTime;
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            textColor.a -= disappearSpeed * Time.deltaTime;
            damageText.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
