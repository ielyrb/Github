using UnityEngine;
using TMPro;

public class CharacterStatsUI : MonoBehaviour
{
    public static CharacterStatsUI instance;

    [Header("Character Info")]
    public TextMeshProUGUI level;
    public TextMeshProUGUI tribe;
    public TextMeshProUGUI job;
    public TextMeshProUGUI caste;
    public TextMeshProUGUI ashram;
    public TextMeshProUGUI god;
    public TextMeshProUGUI masterPoints;
    public TextMeshProUGUI evilPoints;

    [Header("Character Status")]
    public TextMeshProUGUI hp;
    public TextMeshProUGUI tp;
    public TextMeshProUGUI patk;
    public TextMeshProUGUI matk;
    public TextMeshProUGUI pdef;
    public TextMeshProUGUI mdef;
    public TextMeshProUGUI hit;
    public TextMeshProUGUI dodge;
    public TextMeshProUGUI crit;
    public TextMeshProUGUI critEva;
    public TextMeshProUGUI critDmgBoost;
    public TextMeshProUGUI critDmgReduc;
    public TextMeshProUGUI dropChance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(this);
        }
    }
}
