using UnityEngine;
using Mirror;
using TMPro;
using System.Collections;
using System;

public class CommonMonster : NetworkBehaviour
{
    private GameObject targetObj;
    private TantraPlayer target;
    [HideInInspector] public HealthBar healthBar;
    [HideInInspector] [SerializeField] NetworkAnimator netAni;
    [HideInInspector] [SerializeField] Animator ani;
    [HideInInspector] [SerializeField] GameObject floatingDamage;
    [HideInInspector] [SerializeField] Transform attackPoint;
    public ItemDropTable dropTable;
    [HideInInspector] public Transform originalParent;
    [SyncVar] public string targetName;
    private bool hasTarget;
    [SyncVar] public bool canAttack;
    private float attackDistance = 2f;
    private int attackNum;
    private Vector3 originalPos;
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

    [HideInInspector] public TextMeshProUGUI nameText;

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
    public int hit;

    [SyncVar]
    public int dodge;

    [SyncVar]
    public int crit;

    [SyncVar]
    public int criteva;

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
        attackNum = ani.GetInteger("attackNum");
        originalPos = transform.localPosition;
    }

    private void Update()
    {
        if (hasTarget)
        {
            if (canAttack)
            {
                if (isServer)
                {
                    Attack();
                }
            }
        }
        else
        { 
            canAttack = false;
            StopCoroutine(Attacking(1f));
            targetObj = null;
            target = null;
        }
    }

    private void LateUpdate()
    {
        if (target == null || targetObj == null)
        {
            hasTarget = false;
        }
        if (hasTarget)
        {
            if (Vector3.Distance(targetObj.transform.position, transform.position) > attackDistance)
            {
                transform.position = Vector3.Lerp(transform.position, targetObj.transform.position, Time.deltaTime * 1);
            }
            transform.LookAt(targetObj.transform);
        }
        else
        {
            if (isServer)
            {
                ResetPosition();
            }
        }
    }

    IEnumerator GetOriginalPosition()
    {
        yield return new WaitForSeconds(1f);
        originalPos = transform.localPosition;
    }

    void ResetPosition()
    {
        transform.position = Vector3.Lerp(transform.position, originalParent.position, Time.deltaTime * 3);
    }

    public Tuple<int, bool, bool> TakeDamage(TantraPlayer player, string attacker, int amount)
    {
        bool isDodged = false;
        bool isCrit = false;

        if (!hasTarget)
        {
            GetTarget(attacker);
        }

        int damage = amount - pdef;

        if (damage < 0)
        {
            damage = 0;
        }

        #region Check Hit and Crits
        if (CombatSystem.HitAndDodgeSystem(player.hit, dodge))
        {
            damage = 0;
            isDodged = true;
            return new Tuple<int, bool, bool>(damage, isCrit, isDodged);
        }

        Tuple<float, bool> result = CombatSystem.CriticalSystem(player.crit, criteva, player.critDmgBoost, 0);
        damage = (int)Mathf.Round(damage * (1 + result.Item1));
        isCrit = result.Item2;
        #endregion

        curHealth -= damage;
        if (curHealth <= 0)
        {
            enabled = false;
            canAttack = false;
            curHealth = 0;
            StopAllCoroutines();
            StartCoroutine(Die());
            netAni.SetTrigger("isDead");
        }
        else
        {
            //netAni.SetTrigger("isHit");
        }
        return new Tuple<int, bool, bool>(damage, isCrit, isDodged);
    }

    void Attack()
    {
        //attackNum++;
        //if (attackNum == 3)
        //{
        //    attackNum = 1;
        //}
        canAttack = false;
        ani.SetInteger("attackNum", 2);
        netAni.SetTrigger("attack");
        StartCoroutine(Attacking(2f));
    }

    IEnumerator Attacking(float duration)
    {
        yield return new WaitForSeconds(duration);
        canAttack = true;
        float distance = Vector3.Distance(targetObj.transform.position, transform.position);
        if (distance < 2)
        {
            Tuple<int, bool, bool> result = target.TakeDamage(this, patk);
            target.ShowDamageTaken(result.Item1, result.Item2, result.Item3);
            if (target.curHealth <= 0)
            {
                targetName = null;
                hasTarget = false;
                canAttack = false;
                SvrResetHealth();
                netAni.SetTrigger("isReset");
            }
        }
    }

    [Server]
    void SvrResetHealth()
    {
        curHealth = maxHealth;
        RpcResetHealth();
    }

    [ClientRpc]
    void RpcResetHealth()
    {
        healthBar.SetHealth(maxHealth);
    }

    public void OnAttack()
    {
        if (target == null)
        {
            if (targetName == null)
            {
                return;
            }
            GetTarget(targetName);
        }
    }

    [Server]
    void SvrOnAttack()
    {
        canAttack = true;
    }

    void GetTarget(string _name)
    {
        targetObj = GameObject.Find(_name);
        target = targetObj.GetComponent<TantraPlayer>();
        hasTarget = true;
        canAttack = true;
    }

    public int GetHealth()
    {
        return curHealth;
    }

    [ClientRpc]
    public void ShowDamageTaken(int amount, bool isCrit, bool isDodged)
    {
        GameObject obj = Instantiate(floatingDamage, transform.position, Quaternion.identity);

        if (isDodged)
        {
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "MISS";
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
        }
        else
        {
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = amount.ToString();
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
        }

        if (isCrit)
        {
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "CRITICAL " + amount.ToString();
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }

        obj.transform.SetParent(transform.GetChild(2), false);
        obj.transform.localPosition = new Vector3(0, 0, 0);
        Destroy(obj, 2f);
    }

    IEnumerator Die()
    {
        hasTarget = false;
        canAttack = false;
        yield return new WaitForSeconds(2f);
        TantraManager.instance.SpawnCommonMonster(monsterName, level, originalParent);
        Destroy(gameObject);
    }

    [Server]
    void SvrInitMonster()
    {
        maxHealth = 20+15 * level;
        curHealth = maxHealth;
        patk = 20 + 5 * level;
        matk = 20 + 5 * level;
        pdef = 3 * level;
        mdef = 3 * level;
        hit = 60 + 2 * level;
        dodge = 2 * level;
        crit = 4 * level;
        criteva = 2 * level;
        exp = 5 * level;

        name = "Lv. " + level + " " + monsterName.ToString() + " " + GetComponent<NetworkIdentity>().netId;
        RpcInitMonster(name, level, maxHealth, exp);
    }

    [Command(requiresAuthority = false)]
    void CmdInitMonster()
    {
        maxHealth = 20+15 * level;
        curHealth = maxHealth;
        patk = 20 + 5 * level;
        matk = 20 + 5 * level;
        pdef = 3 * level;
        mdef = 3 * level;
        exp = 5 * level;
        hit = 60 + 2 * level;
        dodge = 2 * level;
        crit = 4 * level;
        criteva = 2 * level;
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
