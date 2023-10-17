using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CubeBuild : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private enum POINTEREVENTTYPE
    {
        ENTER,
        EXIT,
        CLICKDOWN,
        CLICKUP
    };

    [Header("Propertied of BuildTile Cube")]
    public Transform towerBuildSiteAnchor = null;
    public GameObject currentTower = null;
    public bool isTowerOnSite = false;
    public bool isUserGonnaBuild = false;
    public Color buildSiteOriginalColor = Color.black;

    [Header("Tower Build Manager")]
    public TowerBuildManager mgr = null;

    private void MarkTowerOnSite(POINTEREVENTTYPE _event)
    {
        // 유저가 타워 건설할 생각이 없다면 == 타워빌드매니저의 건설 상태가 off 일때
        if (!isUserGonnaBuild)
        {
            // 클릭한 큐브에 타워가 있으면 파괴하기
            if (currentTower != null)
            {
                currentTower.transform.SetParent(null);
                Destroy(currentTower);
            }

            // 확인용 큐브 색 잠깐 바꾸기
            gameObject.GetComponent<Renderer>().material.color = buildSiteOriginalColor;
        }

        // 유저가 클릭한 큐브에 타워가 이미 있다면
        if (isTowerOnSite)
        {
            // 마우스 클릭 다운시 건설된 타워를 지운다는 의미로 큐브 색을 파란색으로 바꾸기
            if(_event == POINTEREVENTTYPE.CLICKDOWN)
            {
                gameObject.GetComponent<Renderer>().material.color = Color.blue;
            }
            // 마우스 클릭 업시 타워 철거, 큐브를 원래 색으로 변경, 타워없음으로 플래그 변경을 진행
            else if(_event == POINTEREVENTTYPE.CLICKUP)
            {
                currentTower.transform.SetParent(null);
                Destroy(currentTower);
                gameObject.GetComponent<Renderer>().material.color = buildSiteOriginalColor;
                isTowerOnSite = false;
            }

            return;
        }
        // 유저가 클릭한 큐브에 타워가 없으면 건설 시작
        else
        {
            // 유저가 타워를 세울거다 = 타워빌드매니저의 타워 건설 on 상태
            if (isUserGonnaBuild)
            {
                // 클릭 다운 >> 타워 없으면 세우기
                if (_event == POINTEREVENTTYPE.CLICKDOWN)
                {
                    InstantiateTowerFromManager();

                    // 건설한다 = 큐브 색이 빨간 색
                    gameObject.GetComponent<Renderer>().material.color = Color.red;
                }
                // 클릭 업 == 손을 뗌 == 건설 완료
                else if (_event == POINTEREVENTTYPE.CLICKUP)
                {
                    if (currentTower != null)
                        isTowerOnSite = true;
                    else
                        isTowerOnSite = false;

                    gameObject.GetComponent<Renderer>().material.color = buildSiteOriginalColor;
                }  
            }
        }
    }

    private bool TryBuildTower()
    {
        int currentCredit = mgr.smgr.credit;
        int buildCost = (mgr.GetAttributeOfSelectedTower() != null)
            ? mgr.GetAttributeOfSelectedTower().buildCost : -1;

        if (buildCost < 0)
            return false;

        if (currentCredit - buildCost < 0)
            return false;
        else
        {
            mgr.smgr.GainCredit(buildCost * -1);
            return true;
        }
            
    }

    private void InstantiateTowerFromManager()
    {
        if (!TryBuildTower())
            return;

        if (currentTower == null)
        {
            // 타워빌드매니저를 통해 유저가 선택한 종류의 타워를 큐브의 앵커 위치에 맞춰 생성
            currentTower = Instantiate(mgr.SelectTowerByTowerType(),
                            towerBuildSiteAnchor.transform.position,
                            towerBuildSiteAnchor.transform.rotation);

            // 타워빌드매니저에서 설정한 글로벌 스케일 변수를 참조해서 크기 뻥튀기하기
            float _scale = mgr.towerRenderScale;

            // 빅캐논 타워는 큐브와 같은 에셋이라 괜찮은데 다른 타워는 눈대중으로 스케일 조절할 필요가 있었다
            if (mgr.currentTypeOfTower == BUILDABLETOWERTYPE.BIGCANNON)
                currentTower.transform.localScale *= _scale;
            else
                currentTower.transform.localScale *= (_scale / 1.05f);
        }

        currentTower.transform.SetParent(towerBuildSiteAnchor);
    }

    // ==================================

    private void Awake()
    {
        buildSiteOriginalColor = GetComponent<Renderer>().material.color;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MarkTowerOnSite(POINTEREVENTTYPE.CLICKUP);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MarkTowerOnSite(POINTEREVENTTYPE.CLICKDOWN);
    }
}
