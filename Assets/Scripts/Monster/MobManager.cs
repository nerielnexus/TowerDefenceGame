using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class MobManager : MonoBehaviour
{
    [Header("Mob Spawner UI Elements")]
    public Button enableSpawnerButton = null;

    [Header("Mob Spawner Logic Elements")]
    public MapManager mmgr = null;
    public List<GameObject> monsterList;
    public bool spawnerEnabled = false;
    private bool loopflag_SpawnMonster = false;
    public float mobSpawnInterval = 0.5f;
    public Vector3 spawnLocattion = Vector3.zero;

    [Header("Mob Spawner with Player Logic Elements")]
    public NavMeshPath pathOfPlayer = null;

    [Header("Spawned Monster List")]
    public List<TEST_NavMeshMonster> currentSpawned;

    [Header("Count for Wave type Monster Spawning")]
    public int maxSpawnPerWave = 50;                        // 한 웨이브에서 생성 가능한 최대 몬스터 수
    public float waveInterval = 10f;                        // 웨이브 간격
    public bool waveOnGoing = false;                        // 웨이브 진행 상태 플래그
    public bool turnOnWave = false;                         // 웨이브 모드를 켜는 마스터 플래그, 테스트용

    private IEnumerator Loop_SpawnMonster()
    {
        if(!loopflag_SpawnMonster)
        {
            loopflag_SpawnMonster = true;

            if (!turnOnWave)
                InnerLoop_SpawnMonsterRelentless();
            else
                InnerLoop_SpawnMonsterWave();

            yield return new WaitForSeconds(mobSpawnInterval);

            loopflag_SpawnMonster = false;
        }
    }

    private void InnerLoop_SpawnMonsterRelentless()
    {
        foreach (GameObject mob in monsterList)
        {
            Vector3 spawnPos = Vector3.zero;
            spawnPos.x = mmgr.roadInFlow[0].GetComponent<Renderer>().bounds.center.x;
            spawnPos.y = mmgr.roadInFlow[0].GetComponent<Renderer>().bounds.max.y;
            spawnPos.z = mmgr.roadInFlow[0].GetComponent<Renderer>().bounds.center.z;

            TEST_NavMeshMonster tmp = Instantiate(mob, spawnPos, Quaternion.identity).GetComponent<TEST_NavMeshMonster>();
            tmp.transform.SetParent(transform);

            currentSpawned.Add(tmp);
        }
    }

    private void InnerLoop_SpawnMonsterWave()
    {
        if(!waveOnGoing)
        {

        }
        else
        {

        }
    }

    public void DestroyCurrentSpawnedMobs()
    {
        if(mmgr.cubeStartRoad == null)
        {
            Debug.LogWarning("MobSpawner.DestroyCurrentSpawnedMobs has called but MapManager.cubeStartRoad is null currently");
            return;
        }

        foreach(TEST_NavMeshMonster mob in currentSpawned)
        {
            mob.gameObject.SetActive(false);
            Destroy(mob.gameObject);
        }

        currentSpawned.Clear();
    }

    public void DelistCurrentSpawnedMobs(TEST_NavMeshMonster toDelete)
    {
        currentSpawned.Remove(toDelete);
    }

    private void Awake()
    {
        mmgr = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>()
            ?? throw new System.Exception("MobSpawner has no MapManager");

        currentSpawned = new List<TEST_NavMeshMonster>();

        pathOfPlayer = new NavMeshPath();

        enableSpawnerButton.onClick.AddListener(
            delegate
            {
                loopflag_SpawnMonster = !loopflag_SpawnMonster;
            });
    }

    private void Update()
    {
        if(spawnerEnabled)
            StartCoroutine(Loop_SpawnMonster());

        enableSpawnerButton.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = loopflag_SpawnMonster.ToString();

        mobSpawnInterval = GameManager.Instance.mobSpawnInterval;
    }
}
