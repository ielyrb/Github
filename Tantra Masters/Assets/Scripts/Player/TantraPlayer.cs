using UnityEngine;
using Mirror;
using TMPro;
using Cinemachine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System;

public class TantraPlayer : NetworkBehaviour
{
    public static TantraPlayer instance;
    private Animator ani;
    private NetworkAnimator netAni;
    private bool _isAttacking;
    private bool canRegen = true;
    private int _attackNum;
    private ExpBar expBar = ExpBar.instance;
    public LayerMask enemyLayers;
    [SerializeField] private CinemachineVirtualCamera _camera;
    [SerializeField] private HealthBar healthBar;    
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] GameObject floatingDamage;
    [SerializeField] private GameObject weaponObj;
    [SerializeField] private Transform playerBack;
    [SerializeField] private Transform playerHand;
    [SerializeField] private Transform playerCanvas;
    private Vector3 equippedPos = new Vector3(-0.0204f, 0.0241f, -0.0123f);
    private Quaternion equippedRot = Quaternion.Euler(-21.006f, -136.386f, 130.717f);

    private Vector3 holsterPos;
    private Quaternion holsterRot;

    [SyncVar(hook = nameof(SetPlayerName))]
    public string playerName;

    [SyncVar(hook = nameof(OnMaxHealthChanged))]
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
    public float critDmgBoost;

    [SyncVar]
    public float critDmgReduction;

    [SyncVar]
    public float dropChance;

    [SyncVar(hook=nameof(OnExpChanged))]
    public int exp;

    [SyncVar]
    public int expNextLevel;

    private void Awake()
    {
        ani = GetComponent<Animator>();
        netAni = GetComponent<NetworkAnimator>();
        _attackNum = ani.GetInteger("AttackNum");
        if (playerCanvas == null)
        {
            playerCanvas = transform.GetChild(2).GetChild(0).GetChild(12);
        }
    }

    private void Start()
    {
        if (!isLocalPlayer) return;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        StarterAssets.UICanvasControllerInput.instance.starterAssetsInputs = GetComponent<StarterAssets.StarterAssetsInputs>();
#if UNITY_ANDROID
        MobileDisableAutoSwitchControls.instance.SetState(true, GetComponent<PlayerInput>());
#endif
        _camera.gameObject.SetActive(true);
        PlayerInit();
        StartCoroutine(Regen());
        holsterPos = weaponObj.transform.localPosition;
        holsterRot = weaponObj.transform.localRotation;
        ItemHandler.instance.playerLoaded = true;
    }

    public void PlayerInit()
    {
        playerName = PlayerData.instance.userDataAPI.UserData.userId;
        playerNameText.text = playerName;
        maxHealth = PlayerData.instance.maxHp;
        curHealth = PlayerData.instance.maxHp;
        patk = PlayerData.instance.patk;
        matk = PlayerData.instance.matk;
        pdef = PlayerData.instance.pdef;
        mdef = PlayerData.instance.mdef;
        hit = PlayerData.instance.hit;
        dodge = PlayerData.instance.dodge;
        crit = PlayerData.instance.crit;
        criteva = PlayerData.instance.criteva;
        critDmgBoost = PlayerData.instance.critDmgBoost;
        critDmgReduction = PlayerData.instance.critDmgReduc;
        dropChance = PlayerData.instance.dropChance;
        exp = PlayerData.instance.charInfoAPI.data.exp;
        expNextLevel = PlayerData.instance.charInfoAPI.data.expRequired;
        expBar.SetMaxExp(expNextLevel);
        expBar.SetExp(exp);
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(curHealth);
        CMDInitializePlayer(playerName);
        CMDUpdateStats(0, maxHealth);
        CMDUpdateStats(1, curHealth);
        CMDUpdateStats(2, patk);
        CMDUpdateStats(3, matk);
        CMDUpdateStats(4, pdef);
        CMDUpdateStats(5, mdef);
        CMDUpdateStats(6, exp);
        CMDUpdateStats(7, hit);
        CMDUpdateStats(8, dodge);
        CMDUpdateStats(9, crit);
        CMDUpdateStats(10, criteva);
        CMDUpdateStats(11, critDmgBoost);
        CMDUpdateStats(12, critDmgReduction);
        CMDUpdateStats(13, dropChance);
        PlayerData.instance.UpdateTexts();
    }

    [Command]
    void CmdRegen()
    {
        int regenAmount = (int)Mathf.Round(maxHealth * 0.01f);
        int tempHealth = regenAmount + curHealth;

        if (curHealth == 0) return;
        if (tempHealth >= maxHealth)
        {
            curHealth = maxHealth;
        }
        else
        {
            curHealth += regenAmount;
        }
    }

    IEnumerator Regen()
    {
        if (canRegen)
        {
            yield return new WaitForSeconds(1f);
            if (curHealth == 0) yield break;
            CmdRegen();
            healthBar.SetHealth(curHealth);
            StartCoroutine(Regen());
        }
        else
        {
            yield break;
        }
    }
    
    void Update()
    {
        if (!hasAuthority) return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (curHealth <= 0) return;
            if (_isAttacking) return;
            ChangeWeaponPosition(playerHand);
            _isAttacking = true;
            GetComponent<StarterAssets.ThirdPersonController>().canMove = false;
            netAni.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ButtonHandler.instance.Character();
        }
    }

    void ChangeWeaponPosition(Transform _parent)
    {
        weaponObj.transform.SetParent(_parent, false);
        weaponObj.transform.localPosition = equippedPos;
        weaponObj.transform.localRotation = equippedRot;
    }

    [Command]
    void CMDInitializePlayer(string _name)
    {
        playerName = _name;
        name = _name;
        playerNameText.text = _name;
        RpcInitializePlayer(_name);
    }

    [ClientRpc]
    void RpcInitializePlayer(string _name)
    {
        name = _name;
    }

    [Command]
    void CMDUpdateStats(int type, float amount)
    {
        switch (type)
        { 
            case 0:
                maxHealth = (int) amount;
                break;

            case 1:
                curHealth = (int) amount;
                break;

            case 2:
                patk = (int) amount;
                break;

            case 3:
                matk = (int)amount;
                break;

            case 4:
                pdef = (int)amount;
                break;

            case 5:
                mdef = (int)amount;
                break;

            case 6:
                exp = (int)amount;
                break;

            case 7:
                hit = (int)amount;
                break;

            case 8:
                dodge = (int)amount;
                break;

            case 9:
                crit = (int)amount;
                break;

            case 10:
                criteva = (int)amount;
                break;

            case 11:
                critDmgBoost = amount;
                break;

            case 12:
                critDmgReduction = amount;
                break;

            case 13:
                dropChance = amount;
                break;
        }
    }

    public Tuple<int, bool, bool> TakeDamage(UnityEngine.Object _attacker, int amount)
    {
        int damage = amount - pdef;
        int attackerHit;
        int attackerCrit;
        float attackerCDB;
        bool isCrit = false;
        bool isDodged = false;

        if (_attacker.GetType() == typeof(TantraPlayer))
        {
            var attacker = (TantraPlayer)_attacker;
            attackerHit = attacker.hit;
            attackerCrit = attacker.criteva;
            attackerCDB = attacker.critDmgBoost;
        }
        else
        {
            var attacker = (CommonMonster)_attacker;
            attackerHit = attacker.hit;
            attackerCrit = attacker.criteva;
            attackerCDB = 0.5f;
        }

        if (damage < 0)
        {
            damage = 1;
        }
        #region Check Hit and Crits
        if (CombatSystem.HitAndDodgeSystem(attackerHit, dodge))
        {
            damage = 0;
            isDodged = true;
            return new Tuple<int, bool, bool>(damage, isCrit, isDodged);
        }

        Tuple<float, bool> result = CombatSystem.CriticalSystem(attackerCrit, criteva, attackerCDB, 0);
        damage = (int)Mathf.Round(damage * (1 + result.Item1));
        isCrit = result.Item2;
        #endregion

        curHealth -= damage;
        if (curHealth <= 0)
        {
            RpcDie();
            curHealth = 0;
        }
        else
        {
            //RpcDealDamage();
        }
        return new Tuple<int, bool, bool>(damage, isCrit, isDodged);
    }

    public int GetHealth()
    {
        return curHealth;
    }

    [Command(requiresAuthority = false)]
    void CMDTakeDamage(int damage)
    {
        curHealth -= damage;
        if (curHealth <= 0)
        {
            curHealth = 0;
            netAni.SetTrigger("Die");
        }
        else
        {
            netAni.SetTrigger("isHit");
        }
    }

    public void Dead()
    {
        StartCoroutine(OnDeath());
    }

    IEnumerator OnDeath()
    {
        if (isLocalPlayer)
        {
            Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
            _spawnPoint = NetworkManager.singleton.GetStartPosition();
            yield return new WaitForSeconds(2f);
            canRegen = true;
            transform.position = _spawnPoint.position;
            transform.rotation = _spawnPoint.rotation;
            netAni.SetTrigger("Revived");
            _isAttacking = false;
            StartCoroutine(Regen());
            CmdResetHealth();
            GetComponent<StarterAssets.ThirdPersonController>().canMove = true;
        }
        healthBar.SetHealth(maxHealth);
    }

    [Command]
    void CmdResetHealth()
    {
        curHealth = maxHealth;
    }

    public void UpdateAni()
    {
        if (!hasAuthority) return;
        _attackNum = 2;
        //_attackNum = ani.GetInteger("AttackNum");
        //ani.SetInteger("AttackNum", 2);
        //if (_attackNum == 2)
        //{
        //    ani.SetInteger("AttackNum", 0);
        //}
        //else if (_attackNum == 0)
        //{
        //    ani.SetInteger("AttackNum", 2);
        //}

        float _range = _attackPoint.gameObject.GetComponent<PlayerAttackPoint>().GetRange();
        Collider[] hits = Physics.OverlapSphere(_attackPoint.position, _range, enemyLayers);

        foreach (Collider _hit in hits)
        {
            if (_hit.gameObject.name == name) continue;
            CMDDealDamage(_hit.gameObject.name, patk);
        }
    }

    public void DoneAnimating()
    {
        if (_attackNum < 2) return;
        GetComponent<StarterAssets.ThirdPersonController>().canMove = true;
        _isAttacking = false;

        weaponObj.transform.SetParent(playerBack, false);
        weaponObj.transform.localPosition = holsterPos;
        weaponObj.transform.localRotation = holsterRot;
    }

    [Command]
    void CMDDealDamage(string ID, int amount)
    {
        int targetHealth;
        int _exp = 0;
        int damageDealt = 0;
        int lootId = 0;
        bool isPlayer = false;
        try
        {
            TantraPlayer player = GameObject.Find(ID).GetComponent<TantraPlayer>();
            Tuple<int, bool, bool> result = player.TakeDamage(this, patk);
            targetHealth = player.GetHealth();
            player.ShowDamageTaken(result.Item1, result.Item2, result.Item3);
            isPlayer = true;
        }
        catch
        {
            CommonMonster monster = GameObject.Find(ID).GetComponent<CommonMonster>();
            Tuple<int, bool, bool> result = monster.TakeDamage(this, name, amount);
            damageDealt = result.Item1;
            monster.targetName = name;
            targetHealth = monster.GetHealth();

            monster.ShowDamageTaken(damageDealt, result.Item2, result.Item3);
            _exp = monster.exp;
        }

        if (targetHealth == 0)
        {
            if (!isPlayer)
            {
                exp += _exp;
                ShowFloatingExp(_exp);
                lootId = GameObject.Find(ID).GetComponent<CommonMonster>().dropTable.id;
                GetLoot(lootId);
            }
        }
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
        obj.transform.SetParent(playerCanvas, false);
        obj.transform.localPosition = new Vector3(0, 0, 0);
        Destroy(obj, 2f);
    }

    [ClientRpc]
    void GetLoot(int id)
    {
        if (isLocalPlayer)
        {
            int roll = UnityEngine.Random.Range(0, 100);
            if (roll >= 50)
            {
                ItemDropManager.instance.InitializeLoot(id);
            }
        }
    }

    [ClientRpc]
    void ShowFloatingExp(int _exp)
    {
        if (!hasAuthority) return;
        ExpBar.instance.ShowFloatingExp(_exp);
    }

    [ClientRpc]
    void RpcDealDamage()
    {
        if (!hasAuthority) return;
        netAni.SetTrigger("isHit");
    }

    [ClientRpc]
    void RpcDie()
    {
        if (!hasAuthority) return;
        _isAttacking = true;
        canRegen = false;
        netAni.SetTrigger("Die");
    }

    void OnCurHealthChanged(int oldHealth, int newHealth)
    {
        curHealth = newHealth;
        healthBar.SetHealth(curHealth);
    }

    void OnMaxHealthChanged(int oldHealth, int newHealth)
    {
        maxHealth = newHealth;
        healthBar.SetMaxHealth(newHealth);
    }

    void SetPlayerName(string oldName, string newName)
    {
        playerNameText.text = newName;
        name = playerName;
    }

    void OnExpChanged(int oldVal, int newVal)
    {
        if (isLocalPlayer)
        {
            PlayerData.instance.charInfoAPI.data.exp = newVal;
            PlayerData.instance.UpdateCharInfo(false, false, this);
            if (exp >= PlayerData.instance.charInfoAPI.data.expRequired)
            {
                PlayerData.instance.LevelUp(this);
                curHealth = maxHealth;
                exp = 0;
                expBar.SetExp(0);
                return;
            }
            expBar.SetExp(newVal);
        }
    }

    public void LevelUp(int expRequired)
    {
        expNextLevel = expRequired;
        CmdLevelUp(expRequired);
    }

    [Command]
    void CmdLevelUp(int expRequired)
    {
        exp = 0;
        expNextLevel = expRequired;
        RpcLevelUp(expRequired);
    }

    [ClientRpc]
    void RpcLevelUp(int expRequired)
    {
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(maxHealth);
        if (isLocalPlayer)
        {
            expBar.SetMaxExp(expRequired);
            expBar.SetExp(0);
        }
    }
}
