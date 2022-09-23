using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreenUI;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI loadingText;
    public static PlayerData instance;

    [Space]
    public string userId;
    public int taneys;
    public UserDataAPI userDataAPI;
    public CharInfoAPI charInfoAPI;
    public CharStatsAPI charStatsAPI;
    public InventoryAPI inventoryAPI;
    public EquippedItemAPI equippedAPI;
    bool firstLoad = true;

    public int maxHp, curHp, maxTp, patk, matk, pdef, mdef, hit, dodge, crit, criteva;
    public float critDmgBoost, critDmgReduc, dropChance;

    public int modMaxHp, modMaxTp, modPatk, modMatk, modPdef, modMdef, modHit, modDodge, modCrit, modCritEva;
    public float modPerMaxHp, modPerMaxTp, modPerPatk, modPerMatk, modPerPdef, modPerMdef, modCritDmgBoost, modCritDmgReduc, modDropChance;

    enum TribeNum
    {
        Naga,
        Kimnara,
        Asura,
        Rakshasa,
        Yaksa,
        Gandharva,
        Deva,
        Garuda
    }

    enum JobNum
    {
        Dvanta,
        Banar,
        Satya,
        Druka,
        Karya,
        Nakayuda,
        Vidya,
        Abikara,
        Samabat
    }

    TribeNum tribeNum = new();
    JobNum jobNum = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void LoadData()
    {
        userId = userDataAPI.UserData.userId;
        taneys = userDataAPI.UserData.taneys;
        StartCoroutine(LoadCharInfo(true,null));
    }

    IEnumerator LoadCharInfo(bool isFirstLoad, TantraPlayer player)
    {
        string uri = Globals.characterinfo + "charname=" + userId;
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
            charInfoAPI = new();
            charInfoAPI = JsonConvert.DeserializeObject<CharInfoAPI>(www.downloadHandler.text);
            jobNum = (JobNum)charInfoAPI.data.job;
            tribeNum = (TribeNum)charInfoAPI.data.tribe;
        }
        if (isFirstLoad)
        {
            StartCoroutine(LoadInventoryAndEquipped());
            StartCoroutine(LoadCharStats());
        }
        else
        {
            player.LevelUp(charInfoAPI.data.expRequired);
            ExpBar.instance.SetMaxExp(charInfoAPI.data.expRequired);
            ExpBar.instance.SetExp(charInfoAPI.data.exp);
        }
    }

    IEnumerator LoadInventoryAndEquipped()
    {
        string uri = Globals.inventory + "charname=" + userId + "&limit=24";
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
            inventoryAPI = new();
            inventoryAPI = JsonConvert.DeserializeObject<InventoryAPI>(www.downloadHandler.text);
        }

        uri = Globals.equipped + "charname=" + userId;
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
            equippedAPI = new();
            equippedAPI = JsonConvert.DeserializeObject<EquippedItemAPI>(www.downloadHandler.text);
        }
    }
    IEnumerator LoadCharStats()
    {
        string uri = Globals.characterstats + "charname=" + userId;
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
            charStatsAPI = new();
            charStatsAPI = JsonConvert.DeserializeObject<CharStatsAPI>(www.downloadHandler.text);

            LoadBaseStats();
            LoadLevelStatModifier();
            if (firstLoad)
            {
                LoadScene("Main Game");
                firstLoad = false;
            }
        }
    }

    public void ReloadStats()
    {
        LoadBaseStats();
        LoadLevelStatModifier();
        ResetModifiers();

        foreach (KeyValuePair<string, EquippedItem> kvp in InventoryHandler.instance.equippedItems)
        {
            if (kvp.Value.hasEquipped)
            {
                LoadItemStatModifier(kvp.Value.item);
            }
        }
        //LoadUpdatedStats(null);
    }

    void LoadScene(string _name)
    {
        StartCoroutine(LoadAsynchronously(_name));
    }

    IEnumerator LoadAsynchronously(string _name)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(_name);
        loadingScreenUI.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            progress *= 100;
            loadingText.text = progress.ToString("F0") + "%";
            slider.value = progress;

            yield return null;
        }
    }

    void LoadBaseStats()
    {
        maxHp = charStatsAPI.maxHp;
        curHp = maxHp;
        patk = charStatsAPI.patk;
        matk = charStatsAPI.matk;
        pdef = charStatsAPI.pdef;
        mdef = charStatsAPI.mdef;
        hit = charStatsAPI.hit;
        dodge = charStatsAPI.dodge;
        crit = charStatsAPI.crit;
        criteva = charStatsAPI.criteva;
        critDmgBoost = charStatsAPI.critboost;
        critDmgReduc = charStatsAPI.critreduc;
    }

    void LoadLevelStatModifier()
    {
        maxHp += charInfoAPI.data.charLevel * 20;
        patk += charInfoAPI.data.charLevel * 4;
        matk += charInfoAPI.data.charLevel * 4;
        pdef += charInfoAPI.data.charLevel * 2;
        mdef += charInfoAPI.data.charLevel * 2;
        hit += charInfoAPI.data.charLevel * 2;
        dodge += charInfoAPI.data.charLevel * 2;
        crit += charInfoAPI.data.charLevel * 3;
        criteva += charInfoAPI.data.charLevel * 3;
    }

    void ResetModifiers()
    {
        modMaxHp = 0;
        modPatk = 0;
        modMatk = 0;
        modPdef = 0;
        modMatk = 0;
        modHit = 0;
        modDodge = 0;
        modCrit = 0;
        modCritEva = 0;
        modPerMaxHp = 0;
        modPerMaxTp = 0;
        modPerPatk = 0;
        modPerMatk = 0;
        modPerPdef = 0;
        modPerMdef = 0;
        modCritDmgBoost = 0;
        modCritDmgReduc = 0;
        modDropChance = 0;
    }

    public void LoadItemStatModifier(Item item)
    {
        ModifierStats stats = new();
        stats.modMaxHp += item.stats.flat.hp;
        stats.modMaxTp += item.stats.flat.tp;
        stats.modPatk += item.stats.flat.patk;
        stats.modMatk += item.stats.flat.matk;
        stats.modPdef += item.stats.flat.pdef;
        stats.modMdef += item.stats.flat.mdef;
        stats.modHit += item.stats.flat.hit;
        stats.modDodge += item.stats.flat.dodge;
        stats.modCrit += item.stats.flat.crit;
        stats.modCritEva += item.stats.flat.critEva;

        stats.modPerMaxHp += item.stats.percent.hp;
        stats.modPerMaxTp += item.stats.percent.tp;
        stats.modPerPatk += item.stats.percent.patk;
        stats.modPerMatk += item.stats.percent.matk;
        stats.modPerPdef += item.stats.percent.pdef;
        stats.modPerMdef += item.stats.percent.mdef;
        stats.modCritDmgBoost += item.stats.percent.critDamageBoost;
        stats.modCritDmgReduc += item.stats.percent.critDamageReduc;
        stats.modDropChance += item.stats.percent.dropChance;

        LoadUpdatedStats(stats);
    }

    void LoadUpdatedStats(ModifierStats stats)
    {
        maxHp += stats.modMaxHp;
        maxTp += stats.modMaxTp;
        patk += stats.modPatk;
        matk += stats.modMatk;
        pdef += stats.modPdef;
        mdef += stats.modMdef;
        hit += stats.modHit;
        dodge += stats.modDodge;
        crit += stats.modCrit;
        criteva += stats.modCritEva;
        
        maxHp = (int)Mathf.Round(maxHp * 1 + (stats.modPerMaxHp / 100));
        maxTp = (int)Mathf.Round(maxTp * 1 + (stats.modPerMaxTp / 100));
        patk = (int)Mathf.Round(patk * 1 + (stats.modPerPatk / 100));
        matk = (int)Mathf.Round(matk * 1 + (stats.modPerMatk / 100));
        pdef = (int)Mathf.Round(pdef * 1 + (stats.modPerPdef / 100));
        mdef = (int)Mathf.Round(mdef * 1 + (stats.modPerMdef / 100));
        critDmgBoost += stats.modCritDmgBoost;
        critDmgReduc += stats.modCritDmgReduc;
        dropChance += stats.modDropChance;

        if (TantraPlayer.instance != null)
        {
            TantraPlayer.instance.PlayerInit();
        }
    }

    public void UpdateTexts()
    {
        CharacterStatsUI.instance.hp.text = maxHp.ToString();
        CharacterStatsUI.instance.tp.text = maxTp.ToString();
        CharacterStatsUI.instance.patk.text = patk.ToString();
        CharacterStatsUI.instance.matk.text = matk.ToString();
        CharacterStatsUI.instance.pdef.text = pdef.ToString();
        CharacterStatsUI.instance.mdef.text = mdef.ToString();
        CharacterStatsUI.instance.hit.text = hit.ToString();
        CharacterStatsUI.instance.dodge.text = dodge.ToString();
        CharacterStatsUI.instance.crit.text = crit.ToString();
        CharacterStatsUI.instance.critEva.text = criteva.ToString();
        CharacterStatsUI.instance.critDmgBoost.text = critDmgBoost.ToString() + "%";
        CharacterStatsUI.instance.critDmgReduc.text = critDmgReduc.ToString() + "%";
        CharacterStatsUI.instance.level.text = charInfoAPI.data.charLevel.ToString();
        CharacterStatsUI.instance.tribe.text = tribeNum.ToString();
        CharacterStatsUI.instance.job.text = jobNum.ToString();
    }

    public void LevelUp(TantraPlayer player)
    {
        StartCoroutine(LevelingUp(player));
    }

    IEnumerator LevelingUp(TantraPlayer player)
    {
        charInfoAPI.data.charLevel++;
        charInfoAPI.data.exp = 0;
        UpdateCharInfo(true, false, player);
        ReloadStats();
        //LoadBaseStats();
        //LoadLevelStatModifier();
        //LoadItem
        
        player.maxHealth = maxHp;
        player.curHealth = maxHp;
        charStatsAPI.curHp = maxHp;
        player.PlayerInit();
        UpdateTexts();
        yield return null;
    }

    public void UpdateCharInfo(bool isLeveledUp, bool isFirstLoad, TantraPlayer player)
    {
        string data = JsonConvert.SerializeObject(charInfoAPI);
        StartCoroutine(UpdateCharacterInfo(data, isLeveledUp, isFirstLoad, player));
    }

    IEnumerator UpdateCharacterInfo(string data, bool isLeveledUp, bool isFirstLoad, TantraPlayer player)
    {
        string uri = Globals.updatecharacterinfo + "charname=" + userId + "&data=" + data;
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
        }
        if (!isFirstLoad && isLeveledUp)
        {
            StartCoroutine(LoadCharInfo(false, player));
        }
    }

    class ModifierStats
    {
        public int modMaxHp;
        public int modMaxTp;
        public int modPatk;
        public int modMatk;
        public int modPdef;
        public int modMdef;
        public int modHit;
        public int modDodge;
        public int modCrit;
        public int modCritEva;

        public float modPerMaxHp;
        public float modPerMaxTp;
        public float modPerPatk;
        public float modPerMatk;
        public float modPerPdef;
        public float modPerMdef;
        public float modCritDmgBoost;
        public float modCritDmgReduc;
        public float modDropChance;
    }
}
