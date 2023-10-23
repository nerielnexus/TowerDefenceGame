using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Camera Manager Version 2 (created in 23-10-24)
// goal - delete singleton codes from camera manager v1

/*
 * ���� �����
 * 
 * x
 * value: 1��
 * �̰� �����ؾ���, ���� ���ϸ� ���� ȸ���� �ؼ� ī�޶� ������ �����
 * 
 * y
 * value: 2.3
 * 5*5 8*8 ���� ���簢�� ���� ȭ���� �̻ڰ� ä��� ����
 * 
 * value: 1.3
 * 12*4 16*5 ���� ���η� �� �ʵ��� ȭ���� �̻ڰ� ä��� ����
 * 
 * z
 * value: 0.1
 * ��¦ ���� �༭ �ʹ� ž ��ó�� ������ �ʴ� ����
 * 
 * value: 1
 * �̰� ž��
 * 
 * ���
 * 3*20 16*4 ���� �ش������� ������ ���� ��������
 * ������ ���簢�� ������ �ʸ� �޾ƿ��°ɷ�?
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