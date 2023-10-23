using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Camera Manager Version 2 (created in 23-10-24)
// goal - delete singleton codes from camera manager v1

/*
 * 실측 결과들
 * 
 * x
 * value: 1준
 * 이건 고정해야함, 값이 변하면 맵이 회전을 해서 카메라 밖으로 벗어난다
 * 
 * y
 * value: 2.3
 * 5*5 8*8 같은 정사각형 맵이 화면을 이쁘게 채우는 수준
 * 
 * value: 1.3
 * 12*4 16*5 같은 가로로 긴 맵들이 화면을 이쁘게 채우는 수준
 * 
 * z
 * value: 0.1
 * 살짝 각을 줘서 너무 탑 뷰처럼 보이지 않는 수준
 * 
 * value: 1
 * 이건 탑뷰
 * 
 * 비고
 * 3*20 16*4 같이 극단적으로 길쭉한 맵은 지양하자
 * 당장은 정사각형 형태의 맵만 받아오는걸로?
 */

public class CameraManagerV2 : MonoBehaviour
{
    [Header("Camera")]
    public Camera mainCam = null;
    public Camera mapPreviewCam = null;

    [Header("Camera Position & Rotation")]
    public Vector3 cameraPosition = Vector3.zero;
    public Vector3 cameraLookAtAnchor = Vector3.zero;
    public Vector3 cameraRotation = Vector3.zero;
    public Vector3 cameraScale = Vector3.zero;

    [Header("ETC Elements")]
    public string _currentSceneName = null;

    private void SetCameraViewPriority()
    {

        bool _mobNowSpawning = GameObject.Find("UIManager").GetComponent<InGameUIManager>().mobSpawn;
        bool _userPausedGame = GameObject.Find("UIManager").GetComponent<InGameUIManager>().userPausedGame;

        if (_mobNowSpawning)
        {
            Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("CubeRoad");
            Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("CubeBuild");

            if (mapPreviewCam.gameObject.activeSelf)
                mapPreviewCam.gameObject.SetActive(false);
        }
        else
        {
            if (!_userPausedGame)
            {
                Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("CubeRoad"));
                Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("CubeBuild"));

                if (!mapPreviewCam.gameObject.activeSelf)
                    mapPreviewCam.gameObject.SetActive(true);
            }
        }
    }

    private void SetCameraAnchorInGame()
    {
        InGameUIManager uimgr = GameObject.Find("UIManager").GetComponent<InGameUIManager>();
        MapManager mapmgr = GameObject.Find("MapManager").GetComponent<MapManager>();

        if (!uimgr.userOnSelectMap)
            return;

        if (mapmgr.mapOnList.Length <= 0)
            return;

        float zValue = mapmgr.cubeRoad.GetComponent<Renderer>().bounds.size.z * (mapmgr.mapOnList.Length - 1) / 2;
        float xValue = mapmgr.cubeRoad.GetComponent<Renderer>().bounds.size.x * (mapmgr.mapOnList[0].Length - 1) / 2;

        Vector3 previewAnchor = new Vector3(xValue, 0, zValue);
        mapPreviewCam.transform.localPosition = new Vector3(xValue, (xValue > zValue ? xValue : zValue), zValue);
        mapPreviewCam.transform.LookAt(previewAnchor);
        mapPreviewCam.transform.rotation *= Quaternion.Euler(cameraRotation);
        mapPreviewCam.orthographic = true;
        mapPreviewCam.orthographicSize = Mathf.Ceil(xValue > zValue ? xValue : zValue) + 10;

        bool towerBuildMode = GameObject.FindGameObjectWithTag("TowerBuildManager")
            ? GameObject.FindGameObjectWithTag("TowerBuildManager").GetComponent<TowerBuildManager>().modeTowerBuild : false;

        cameraLookAtAnchor = new Vector3(xValue, 0, zValue);
        cameraPosition = Vector3.Scale(new Vector3(xValue, (xValue > zValue ? xValue : zValue), zValue), towerBuildMode ? cameraScale + new Vector3(0, 1, 0) : cameraScale);
        mainCam.transform.position = cameraPosition;
        mainCam.transform.LookAt(cameraLookAtAnchor);

        SetCameraViewPriority();
    }

    private void AddComponentToMainCamera()
    {
        if (mainCam.GetComponent<UnityEngine.EventSystems.PhysicsRaycaster>() == null)
            mainCam.gameObject.AddComponent<UnityEngine.EventSystems.PhysicsRaycaster>();

        mainCam.GetComponent<UnityEngine.EventSystems.PhysicsRaycaster>().eventMask = LayerMask.GetMask("CubeBuild");
    }

    //////////////////////////////////////////

    private void Awake()
    {
        mainCam = Camera.main;

        _currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if(_currentSceneName.Equals("SceneGamePlay"))
        {
            mapPreviewCam = GameObject.Find("Map Preview Camera V2").GetComponent<Camera>() ??
                throw new System.Exception(nameof(CameraManagerV2) + " cannot find map preview camera in gameplay scene");
        }
    }

    private void Update()
    {
        AddComponentToMainCamera();

        if (_currentSceneName.Equals("SceneGamePlay"))
            SetCameraAnchorInGame();
    }
}