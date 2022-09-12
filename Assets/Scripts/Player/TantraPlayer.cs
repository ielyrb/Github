using UnityEngine;
using Mirror;
using TMPro;
using Cinemachine;
using UnityEngine.InputSystem;
using System.Collections;

public class TantraPlayer : NetworkBehaviour
{
    private Animator ani;
    private NetworkAnimator netAni;
    private bool _isAttacking;
    private int _attackNum;
    private ExpBar expBar = ExpBar.instance;
    public LayerMask enemyLayers;
    [SerializeField] private CinemachineVirtualCamera _camera;
    [SerializeField] private HealthBar healthBar;    
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private TextMeshProUGUI playerNameText;

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

    [SyncVar(hook=nameof(OnExpChanged))]
    public int exp;

    [SyncVar]
    public int expNextLevel;

    private void Awake()
    {
        ani = GetComponent<Animator>();
        netAni = GetComponent<NetworkAnimator>();
        _attackNum = ani.GetInteger("AttackNum");
    }

    private void Start()
    {
        if (!isLocalPlayer) return;
        StarterAssets.UICanvasControllerInput.instance.starterAssetsInputs = GetComponent<StarterAssets.StarterAssetsInputs>();
#if UNITY_ANDROID
        MobileDisableAutoSwitchControls.instance.SetState(true, GetComponent<PlayerInput>());
#endif
        _camera.gameObject.SetActive(true);
        PlayerInit();
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
        exp = PlayerData.instance.charInfoAPI.data.exp;
        expNextLevel = PlayerData.instance.charInfoAPI.data.expRequired;
        expBar.SetMaxExp(PlayerData.instance.charInfoAPI.data.expRequired);
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
        PlayerData.instance.UpdateTexts();
    }
    
    void Update()
    {
        if (!hasAuthority) return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (_isAttacking) return;
            _isAttacking = true;
            GetComponent<StarterAssets.ThirdPersonController>().canMove = false;
            netAni.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ButtonHandler.instance.Character();
        }
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
    void CMDUpdateStats(int type, int amount)
    {
        switch (type)
        { 
            case 0:
                maxHealth = amount;
                break;

            case 1:
                curHealth = amount;
                break;

            case 2:
                patk = amount;
                break;

            case 3:
                matk = amount;
                break;

            case 4:
                pdef = amount;
                break;

            case 5:
                mdef = amount;
                break;

            case 6:
                exp = amount;
                break;
        }
    }

    public int TakeDamage(int damage)
    {
        curHealth -= damage;
        if (curHealth <= 0)
        {
            RpcDie();
            curHealth = 0;
        }
        else
        {
            RpcDealDamage();
        }
        return curHealth;
    }

    void Die()
    {
        netAni.SetTrigger("Die");
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
        Destroy(gameObject);
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

        foreach (Collider hit in hits)
        {
            if (hit.gameObject.name == name) continue;
            CMDDealDamage(hit.gameObject.name, patk);
        }
    }

    public void DoneAnimating()
    {
        if (_attackNum < 2) return;
        GetComponent<StarterAssets.ThirdPersonController>().canMove = true;
        _isAttacking = false;
    }

    [Command]
    void CMDDealDamage(string ID, int amount)
    {
        int targetHealth;
        int _exp = 0;
        try
        {
            targetHealth = GameObject.Find(ID).GetComponent<TantraPlayer>().TakeDamage(amount);
        }
        catch
        {
            targetHealth = GameObject.Find(ID).GetComponent<CommonMonster>().TakeDamage(name,amount);
            _exp = GameObject.Find(ID).GetComponent<CommonMonster>().exp;
        }

        if (targetHealth == 0)
        {
            exp += _exp;
        }
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
        Debug.Log(expRequired);
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
