using UnityEngine;
using Mirror;
using TMPro;
using System.Collections;

public class CommonMonster : NetworkBehaviour
{
    public HealthBar healthBar;
    [SerializeField] NetworkAnimator netAni;
    [SerializeField] Animator ani;
    public enum MonsterType
    { 
        Field,
        Dungeon,
        Raid,
        Special
    }

    public enum MonsterName
    { 
        Slime
    }

    public TextMeshProUGUI nameText;

    public MonsterName monsterName;
    public MonsterType monType;

    [SyncVar]
    public int level;

    [SyncVar]
    public int maxHealth;

    [SyncVar(hook = nameof(OnCurHealthChanged))]
    public int curHealth;

    [SyncVar]
    public int patk;

    [SyncVar]
    public int matk;

    [SyncVar]
    public int pdef;

    [SyncVar]
    public int mdef;

    [SyncVar]
    public int exp;

    private void Start()
    {
        if (isServer)
        {
            SvrInitMonster();
        }
        else
        {
            CmdInitMonster();
        }

        ani.SetBool("isIdling", true);
    }

    public int TakeDamage(string attacker, int amount)
    {
        curHealth -= amount;
        if (curHealth <= 0)
        {
            curHealth = 0;
            StartCoroutine(Die());
            netAni.SetTrigger("isDead");
        }
        else
        {
            netAni.SetTrigger("isHit");
        }
        return curHealth;
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(2f);
        TantraManager.instance.SpawnCommonMonster(monsterName, level, transform.parent);
        Destroy(gameObject);
    }

    [Server]
    void SvrInitMonster()
    {
        maxHealth = 20 * level;
        curHealth = maxHealth;
        patk = 5 * level;
        matk = 5 * level;
        pdef = 5 * level;
        mdef = 5 * level;
        exp = 5 * level;
        name = "Lv. " + level + " " + monsterName.ToString() + " " + GetComponent<NetworkIdentity>().netId;
        RpcInitMonster(name, level, maxHealth, exp);
    }

    [Command(requiresAuthority = false)]
    void CmdInitMonster()
    {
        maxHealth = 20 * level;
        curHealth = maxHealth;
        patk = 5 * level;
        matk = 5 * level;
        pdef = 5 * level;
        mdef = 5 * level;
        exp = 5 * level;

        RpcInitMonster(name, level, maxHealth, exp);
    }

    [ClientRpc]
    void RpcInitMonster(string _name, int _level, int _maxhealth, int _exp)
    {
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(maxHealth);
        nameText.text = "Lv. " + level + " " + monsterName.ToString();
        name = "Lv. " + level + " " + monsterName.ToString() + " " + GetComponent<NetworkIdentity>().netId;
    }

    void OnCurHealthChanged(int oldHealth, int newHealth)
    {
        if (isServer)
        {
            SvrChangeCurHealth(newHealth);
        }
        else
        {
            CmdChangeCurHealth(newHealth);
        }
    }

    [Server]
    void SvrChangeCurHealth(int newValue)
    {
        RpcChangeCurHealth(newValue);
    }

    [Command(requiresAuthority = false)]
    void CmdChangeCurHealth(int newValue)
    {
        RpcChangeCurHealth(newValue);
    }

    [ClientRpc]
    void RpcChangeCurHealth(int newValue)
    {
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(newValue);
    }
}
