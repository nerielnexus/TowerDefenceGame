using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

// 몬스터 객체를 start cube 위에 생성하거나, active 상태인 몬스터 객체를 전부 지우는 로직을
// 버튼 ui 에 할당해 사용할 수 있게 하는 스크립트

public class TEST_SpawnMonster : MonoBehaviour
{
    public GameObject monster = null;
    public bool modeDelete = false;

    private void Awake()
    {
        List<CubeRoad> waypointList = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>().roadInFlow;

        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            if(!modeDelete)
            {
                Vector3 spawnPos = Vector3.zero;
                spawnPos.x = waypointList[0].GetComponent<Renderer>().bounds.center.x;
                spawnPos.y = waypointList[0].GetComponent<Renderer>().bounds.max.y;
                spawnPos.z = waypointList[0].GetComponent<Renderer>().bounds.center.z;

                Instantiate(monster, spawnPos, Quaternion.identity);
            } 
            else
            {
                TEST_NavMeshMonster[] deleteList = FindObjectsOfType<TEST_NavMeshMonster>();
                foreach (TEST_NavMeshMonster toDelete in deleteList)
                    Destroy(toDelete.gameObject);
            }
        });
    }
}
