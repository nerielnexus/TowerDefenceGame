using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;


public class MapManager : MonoBehaviour
{
    // ============================================
    /// <summary>
    /// UnityWebRequest 를 이용해 로컬 저장소에서 파일 읽어오기
    /// </summary>
    /// 

    /*
     * reference for this script
     * 
     * https://theslidefactory.com/using-unitywebrequest-to-download-resources-in-unity/
     * 
     * https://luv-n-interest.tistory.com/1331
     * (action, func keyword)
     * => func: pre-made delegate variable with value returns
     * => action: pre-made delegate variable w/o value returns
     * 
     */

    [Header("Load File (UnityWebRequest)")]
    private string fileUrl = null;
    public string[] mapOnList = null;

    private IEnumerator GetRequest(string url, System.Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            callback(request);
        }
    }

    private void GetPosts()
    {
        StartCoroutine(GetRequest(fileUrl, (UnityWebRequest req) =>
            {
                // if(req.isNetworkError || req.isHttpError)
                if(req.result == UnityWebRequest.Result.ConnectionError||req.result == UnityWebRequest.Result.ProtocolError)
                {
                    // 해당 error 가 발생한다면 여기서 처리할 것
                }
                else
                {
                    // 위의 error 가 발생하지 않고 정상적으로 동작했다면 이후의 처리를 여기서 할 것
                    if (mapOnList != null)
                        mapOnList = null;

                    mapOnList = req.downloadHandler.text.Split('\n');
                    OnButtonClick_InstantiateMap();
                }
            })
        );
    }

    // ============================================
    public void LoadMapFromDropDownMenu(TMPro.TMP_Dropdown mapDropdown)
    {
        if(mapDropdown.options.Count <= 0)
        {
            // 로컬 저장소에 맵 파일이 없다면 그에 맞는 처리를 하는 블록
            return;
        }

        // 현재 드롭다운 UI 를 이용해 몇 번째 드롭다운 메뉴를 선택했는지 확인하고
        // 그에 맞는 index 값을 구해 드롭다운에서 텍스트를 뽑아냄
        int ddIdx = mapDropdown.value;
        string currentOptionText = mapDropdown.options[ddIdx].text;

        // 뽑아낸 텍스트를 이용해 로컬 저장소의 주소를 설정하고, 해당 위치의 파일을 읽어옴
        fileUrl = Application.streamingAssetsPath + "/" + currentOptionText;
        GetPosts();
    }

    // ============================================
    /// <summary>
    /// cube Prefab 을 이용해 맵 만들기
    /// </summary>

    // 맵 로직에 사용할 요소들
    [Header("Elements for Create Map")]
    public GameObject cubeRoad = null;
    public GameObject cubeBuild = null;
    public GameObject mapMotherObject = null;
    public GameObject cubeStartRoad = null;
    public GameObject cubeEndRoad = null;
    public GameObject cubeBeforeEndRoad = null;
    public List<NavMeshSurface> surfaceList = new List<NavMeshSurface>();

    // 맵 생성 전 '맵의 모체' 를 생성
    // '모체' 에 실제로 사용할 맵인 '큐브' 를 붙여서 맵을 만든다
    private void InitializeMapObject()
    {
        // '모체' 가 있으면 진행 ㄴ
        if (mapMotherObject)
            return;

        // '모체' 생성 후, 자식 개체로 'Road' 와 'Build' 추가
        // 'Road', 'Build' 에 대응하는 유형의 큐브를 자식 개체로 붙여 맵을 만들거임
        mapMotherObject = new GameObject();
        mapMotherObject.transform.position = Vector3.zero;
        mapMotherObject.name = "Current Map Object";

        // 맵 모체의 하위 객체로 Roads, Build Site, ETC 를 생성
        // 각각 몬스터가 지나갈 길 큐브, 유저가 타워를 설치할 건설큐브, 기타 등등 큐브를 가리킴
        GameObject mapChildRoad = new GameObject();
        mapChildRoad.transform.position = Vector3.zero;
        mapChildRoad.transform.SetParent(mapMotherObject.transform);
        mapChildRoad.name = "Roads";
        mapChildRoad.transform.SetSiblingIndex(0);

        GameObject mapChildBuild = new GameObject();
        mapChildBuild.transform.position = Vector3.zero;
        mapChildBuild.transform.SetParent(mapMotherObject.transform);
        mapChildBuild.name = "Build Site";

        GameObject mapchildETC = new GameObject();
        mapchildETC.transform.position = Vector3.zero;
        mapchildETC.transform.SetParent(mapMotherObject.transform);
        mapchildETC.name = "ETC";
    }

    public void InstantiateMapCubes()
    {
        Transform roads = mapMotherObject.transform.GetChild(0);
        Transform builds = mapMotherObject.transform.GetChild(1);
        Transform etc = mapMotherObject.transform.GetChild(2);

        // 맵을 만들 때 각 큐브의 좌표와 스케일을 설정하기 위해 값을 미리 받아옴
        // Renderer.bounds 의 size 등 값들은 scale (1, 1, 1) 기준으로 돌아가기 때문에
        // scale 은 따로 받아올 필요가 있다
        Vector3 localScale = cubeBuild.transform.localScale;
        Vector3 localPosition = mapMotherObject.transform.position;
        Vector3 localSize = cubeBuild.GetComponent<Renderer>().bounds.size;

        // 위에서 받아온 맵 배열 (mapOnList) 를 for 루프로 돌리면서
        // 맵을 위치에 맞게, 조건에 맞게(1:build 2:road) instantiate 한다
        for(int _iterZ = 0; _iterZ < mapOnList.Length; _iterZ++)
        {
            for(int _iterX = 0; _iterX < mapOnList[_iterZ].Length; _iterX++)
            {
                if(mapOnList[_iterZ][_iterX] == '1')
                {
                    // build 큐브를 놓는 로직
                    GameObject tmp = Instantiate(cubeBuild, builds);
                    tmp.transform.position = new Vector3(localPosition.x + localSize.x * _iterX, localPosition.y + 0.3f, localPosition.z + localSize.z * _iterZ);
                    tmp.transform.localScale = localScale;

                    // build 큐브의 hierarchy 상 이름을 설정함
                    tmp.name = "build (" + _iterX + ", " + _iterZ + ")";

                    // build 큐브의 build Anchor 위치를 설정해줌
                    Vector3 anchorPos = new Vector3(
                        tmp.GetComponent<Renderer>().bounds.center.x,
                        tmp.GetComponent<Renderer>().bounds.max.y,
                        tmp.GetComponent<Renderer>().bounds.center.z
                        );
                    tmp.transform.GetChild(0).position = anchorPos;
                }
                else if(mapOnList[_iterZ][_iterX] == '0')
                {
                    // road 큐브를 놓는 로직
                    GameObject tmp = Instantiate(cubeRoad, roads);
                    tmp.transform.position = new Vector3(localPosition.x + localSize.x * _iterX, localPosition.y, localPosition.z + localSize.z * _iterZ);
                    tmp.transform.localScale = localScale;

                    // road 큐브의 identifier 를 일반적인 road 큐브로 설정함
                    tmp.GetComponent<CubeRoad>().identifier = CubeRoad.ROADIDTYPE.ROAD;

                    // road 큐브의 hierarchy 상 이름을 설정함
                    tmp.name = "road (" + _iterX + ", " + _iterZ + ")";
                }
                else if (mapOnList[_iterZ][_iterX] == '2')
                {
                    // 몬스터가 스폰되는, start 역할을 하는 road 큐브를 놓는 로직
                    GameObject tmp = Instantiate(cubeRoad, roads);
                    tmp.transform.position = new Vector3(localPosition.x + localSize.x * _iterX, localPosition.y, localPosition.z + localSize.z * _iterZ);
                    tmp.transform.localScale = localScale;

                    // 테스트를 위해 start road 큐브는 초록색으로 나옴
                    tmp.GetComponent<Renderer>().material.color = Color.green;

                    // start road 를 map manager 의 start cube 에 할당함
                    cubeStartRoad = tmp;

                    // road 큐브의 identifier 를 start road 큐브로 설정함
                    tmp.GetComponent<CubeRoad>().identifier = CubeRoad.ROADIDTYPE.START;

                    // road 큐브의 hierarchy 상 이름을 설정함
                    tmp.name = "road start (" + _iterX + ", " + _iterZ + ")";
                }
                else if (mapOnList[_iterZ][_iterX] == '3')
                {
                    // 몬스터의 목적지인, end 역할을 하는 road 큐브를 놓는 로직
                    GameObject tmp = Instantiate(cubeRoad, roads);
                    tmp.transform.position = new Vector3(localPosition.x + localSize.x * _iterX, localPosition.y, localPosition.z + localSize.z * _iterZ);
                    tmp.transform.localScale = localScale;

                    // 테스트를 위해 end road 큐브는 빨간색으로 나옴
                    tmp.GetComponent<Renderer>().material.color = Color.red;

                    // end road 를 map manager 의 end cube 에 할당함
                    cubeEndRoad = tmp;

                    // road 큐브의 identifier 를 end road 큐브로 설정함
                    tmp.GetComponent<CubeRoad>().identifier = CubeRoad.ROADIDTYPE.END;

                    // road 큐브의 hierarchy 상 이름을 설정함
                    tmp.name = "road end (" + _iterX + ", " + _iterZ + ")";
                }

                else if(mapOnList[_iterZ][_iterX] == '4')
                {
                    // 맵의 빈 공간을 표현함
                    GameObject tmp = new GameObject();
                    tmp.transform.parent = etc;
                    tmp.transform.position = new Vector3(localPosition.x + localSize.x * _iterX, localPosition.y, localPosition.z + localSize.z * _iterZ);
                    tmp.transform.localScale = localScale;

                    tmp.name = "etc empty (" + _iterX + ", " + _iterZ + ")";
                }
            }
        }



        // 길 큐브가 자신과 인접한 다른 길 큐브를 찾아서 저장하도록 시킴
        // 이중 연결 리스트가 자신의 전/후 노드에 대한 정보를 갖고 있다는 것에서 착안함
        for(int i=0; i<mapMotherObject.transform.GetChild(0).childCount; i++)
        {
            mapMotherObject.transform.GetChild(0).GetChild(i).GetComponent<CubeRoad>().GetNearbyRoadCubes();
        }

        // 길 큐브의 순서를 확인하고 저장함
        CreateRoadInFlow();

        // CreateRoadInFlow 메서드를 통해 cuberoad 들의 흐름 순서를 생성했다면
        // 그 순서를 참조해 cuberoad 의 navmesh 를 생성한다
        // 추후 area cost 를 이용한 나만의 pathfinding 에 사용할 수 있도록 roadinflow 의 순서에
        // 따라 for loop 을 돌도록 작성했다
        for(int i=0; i<roadInFlow.Count; i++)
        {
            roadInFlow[i].GetComponent<NavMeshSurface>().RemoveData();

            if(i == roadInFlow.Count -1)
            {
                roadInFlow[i].GetComponent<NavMeshSurface>().defaultArea = 5;
            }
            else if(i == roadInFlow.Count -2)
            {
                roadInFlow[i].GetComponent<NavMeshSurface>().defaultArea = 4;
            }
            else
            {
                roadInFlow[i].GetComponent<NavMeshSurface>().defaultArea = 3;
            }

            roadInFlow[i].GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }

    public void ResetCurrentMap()
    {
        // 새 맵을 불러오기 전 기존의 맵을 지우는 작업을 함

        Transform roads = mapMotherObject.transform.GetChild(0);
        Transform builds = mapMotherObject.transform.GetChild(1);

        NavMesh.RemoveAllNavMeshData();

        if (roads.childCount > 0)
        {
            // road 큐브가 하나라도 있다면 삭제하자
            foreach (Transform toDelete in roads)
            {
                toDelete.GetComponent<NavMeshSurface>().RemoveData();
                toDelete.gameObject.SetActive(false);
                Destroy(toDelete.gameObject);
            }  
        }

        if (builds.childCount > 0)
        {
            // build 큐브가 하나라도 있다면 삭제하자
            foreach (Transform toDelete in builds)
                Destroy(toDelete.gameObject);
        }

        // NavMeshSurface 정보를 삭제한다
        surfaceList.Clear();
    }

    private void OnButtonClick_InstantiateMap()
    {
        // 맵 생성하기 버튼을 누르면 처리할 동작들
        GameObject.FindGameObjectWithTag("MobManager").GetComponent<MobManager>().DestroyCurrentSpawnedMobs();
        ResetCurrentMap();
        InstantiateMapCubes();
    }

    // ============================================
    /// <summary>
    /// map manager 가 갖고 있는 road 하위 객체들을 하나의 길로 이어질 수 있게 설정한다
    /// 확인하는 순서는 반시계방향
    /// </summary>    

    [Header("Road-in-Flow")]
    public List<CubeRoad> roadInFlow = null;
    private CubeRoad roadPrev = null;
    public bool mapLoopFlag = false;

    private void CreateRoadInFlow()
    {
        // map mother object(맵 모체)가 없다 = 맵이 생성되기 전이므로 길 큐브 흐름도 생성하지 않게 설정
        if (!mapMotherObject) return;

        // 맵 모체가 있고, 이전에 생성한 맵이 있다면 초기화하기
        // 하는김에 맵의 loop 여부를 확인하는 boolean 변수도 초기화하기
        if (roadInFlow.Count > 0)
        {
            roadInFlow.Clear();
            mapLoopFlag = false;
        }

        // while loop 관리용 플래그
        // 길 큐브를 따라 iterator 가 쭉 이동하다가 identifier 가 end 인 큐브를 만나면 true 로 변경됨
        bool reachedeEnd = false;

        // start 길 큐브를 길 큐브 흐름도에 등록함, 왜냐면 그게 첫번째인 start 큐브니까
        CubeRoad iter = cubeStartRoad.GetComponent<CubeRoad>();
        roadInFlow.Add(iter);

        while (!reachedeEnd)
        {
            // 모든 큐브는 자신 주변 (전후좌우) 길 큐브에 대한 정보를 저장할 수 있음
            // 그 정보에 접근해서 길 큐브 흐름도를 생성하는 것
            foreach (CubeRoad nearby in iter.nearbyCubes)
            {
                if (nearby == null)
                    continue;

                // 맵 파일 작성 규칙 중 하나, 계속 뺑뺑이 도는 loop 형태의 맵은 start 와 end 가 붙어있다
                // 주변 길 큐브 정보를 확인해서, 만약 start 와 end 가 붙어있는게 확인되면 loop 맵으로 설정한다
                if (iter.identifier == CubeRoad.ROADIDTYPE.START && nearby.identifier == CubeRoad.ROADIDTYPE.END)
                {
                    mapLoopFlag = true;
                    continue;
                }

                // 자신이 start 가 아니면서 인접 길 큐브가 end 라면 도착하기 직전이라는 의미
                // end 길 큐브를 등록한 후 길 큐브 흐름도 생성 과정을 종료한다
                if (iter.identifier != CubeRoad.ROADIDTYPE.START && nearby.identifier == CubeRoad.ROADIDTYPE.END)
                {
                    reachedeEnd = true;
                    // roadInFlow.Add(iter);
                    roadInFlow.Add(nearby);
                    break;
                }

                // 자신과 인접 길 큐브가 start, end 가 아니라면 그냥 길 큐브들이 이어지는 상황이다
                // iterator 가 이전 길 큐브(=roadPrev) 로 돌아가지 않도록 조건을 설정하고 iterator 를 한 칸 전진시킨다
                //
                // roadPrev == null 은 start 길 큐브에서 처음으로 iterator 를 한 칸 전진시킬 때 확인하는 조건이다
                if (!nearby.Equals(roadPrev) || roadPrev == null)
                {
                    roadPrev = iter;
                    iter = nearby;
                    roadInFlow.Add(iter);
                    break; 
                }
            } 
        }
    }

    // ============================================

    private void Awake()
    {
        // '맵 모체' 객체를 생성한다
        InitializeMapObject();
    }
}
