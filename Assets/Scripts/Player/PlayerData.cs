using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;
    public string userId;
    public int taneys;
    public UserDataAPI userDataAPI;
    public CharInfoAPI charInfoAPI;
    public CharStatsAPI charStatsAPI;
    bool firstLoad = true;

    public int maxHp, curHp, tp, patk, matk, pdef, mdef, hit, dodge, crit, criteva;

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
            StartCoroutine(LoadCharStats());
        }
        else
        {
            player.LevelUp(charInfoAPI.data.expRequired);
            ExpBar.instance.SetMaxExp(charInfoAPI.data.expRequired);
            ExpBar.instance.SetExp(charInfoAPI.data.exp);
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
            LoadItemStatModifier();
            if (firstLoad)
            {
                SceneManager.LoadScene("Main Game");
                firstLoad = false;
            }
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

    void LoadItemStatModifier()
    { 
        //coming soon
    }

    public void UpdateTexts()
    {
        CharacterStatsUI.instance.hp.text = charStatsAPI.curHp.ToString() + " / " + maxHp.ToString();
        CharacterStatsUI.instance.tp.text = charStatsAPI.mp.ToString();
        CharacterStatsUI.instance.patk.text = patk.ToString();
        CharacterStatsUI.instance.matk.text = matk.ToString();
        CharacterStatsUI.instance.pdef.text = pdef.ToString();
        CharacterStatsUI.instance.mdef.text = mdef.ToString();
        CharacterStatsUI.instance.hit.text = hit.ToString();
        CharacterStatsUI.instance.dodge.text = dodge.ToString();
        CharacterStatsUI.instance.crit.text = crit.ToString();
        CharacterStatsUI.instance.critEva.text = criteva.ToString();
        CharacterStatsUI.instance.critDmgBoost.text = charStatsAPI.critboost.ToString() + "%";
        CharacterStatsUI.instance.critDmgReduc.text = charStatsAPI.critreduc.ToString() + "%";
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
        LoadBaseStats();
        LoadLevelStatModifier();
        
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
}
