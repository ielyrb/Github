using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;

public class TantraManager : NetworkManager
{
    public static TantraManager instance;
    public GameObject[] ClientUI;
    public bool serverBuild;
    public string playerName;

    public List<GameObject> enemyPrefab;

    public List<Transform> enemySpawnAreas;
    public override void OnStartServer()
    {
        base.OnStartServer();
        Camera.main.gameObject.SetActive(false);
        if (serverBuild)
        {
            StartCoroutine(SpawnersInit());
        }
        else
        {
            //StartCoroutine(FixMonsterParent());
        }
    }

    IEnumerator SpawnersInit()
    {
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Monster Spawner");
        foreach (GameObject obj in spawners)
        {
            foreach (Transform child in obj.transform)
            {
                enemySpawnAreas.Add(child);
            }
        }

        yield return new WaitForSeconds(1f);

        foreach (Transform pos in enemySpawnAreas)
        {
            GameObject enemy = Instantiate(enemyPrefab[pos.GetComponentInParent<MonsterSpawner>().level], pos.position, pos.rotation);
            enemy.transform.SetParent(pos.transform, false);
            enemy.GetComponent<CommonMonster>().originalParent = enemy.transform.parent;
            NetworkServer.Spawn(enemy);
            enemy.transform.SetParent(enemy.transform.parent.parent.parent, false);
        }
    }

    private void OnEnable()
    {
        if (!serverBuild)
        {
            StartClient();
        }
        else
        {
            StartServer();
        }
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        //if (!serverBuild) return;
        //foreach (Transform pos in enemySpawnAreas)
        //{
        //    if (pos.childCount == 0)
        //    {
        //        GameObject enemy = Instantiate(enemyPrefab, pos.position, pos.rotation);
        //        enemy.transform.SetParent(pos.transform, false);
        //        NetworkServer.Spawn(enemy);            
        //    }
        //}
    }

    public void SpawnCommonMonster(CommonMonster.MonsterName monName, int level, Transform pos)
    {
        if (!serverBuild) return;
        int monNum = 0;

        if (level == 1)
        {
            monNum = 0;
        }

        if (level == 10)
        {
            monNum = 1;
        }

        if (level == 20)
        {
            monNum = 2;
        }

        if (level == 50)
        {
            monNum = 3;
        }

        StartCoroutine(SpawnComMonster(monNum, pos));
    }

    IEnumerator SpawnComMonster(int monsterId, Transform pos)
    {
        yield return new WaitForSeconds(5f);
        GameObject enemy = Instantiate(enemyPrefab[monsterId], pos.position, pos.rotation);
        enemy.transform.SetParent(pos.transform, false);
        enemy.GetComponent<CommonMonster>().originalParent = pos;
        NetworkServer.Spawn(enemy); 
        enemy.transform.SetParent(enemy.transform.parent.parent.parent, false);
    }
}
