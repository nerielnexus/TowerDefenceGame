using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    [Header("Flags")]
    public bool mobSpawn = false;
    public bool userPausedGame = false;
    public bool userOnSelectMap = false;

    [Header("Other Managers")]
    public CameraManagerV2 camMgr = null;
    public MapManager mapMgr = null;
    public MobManager mobMgr = null;
    public ScoreManager scMgr = null;
    public TowerBuildManager tbMgr = null;

    [Header("UI Elements - Playing Game Canvas")]
    public Button buttonPauseGame = null;
    public GameObject pauseMenu = null;
    public Button buttonChangeToFPSView = null;
    public GameObject canvasTopview = null;

    [Header("UI Elements - Select Stage Canvas")]
    public TMPro.TMP_Dropdown mapDropdown = null;
    public Button buttonLoadFromDevice = null;
    public Button buttonStartGame = null;
    public Button buttonSelectToTitle = null;
    public GameObject canvasSelectStage = null;

    [Header("UI Elements - Cheat Panel")]
    public Button buttonCheatPanel = null;
    public GameObject canvasCheatPanel = null;

    [Header("UI Elements - Select Stage Canvas, flavor text")]
    public TMPro.TMP_Text stageNameText = null;
    public TMPro.TMP_Text stageInfoText = null;
    public TMPro.TMP_Text stageFlavorText = null;

    [Header("UI Elements - First Person View")]
    public bool isFPS = false;
    public GameObject canvasFPS = null;
    public Button buttonChangeToTopview = null;
    public Button buttonPauseGameinFPS = null;

    private void InitializeElements()
    {
        camMgr = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManagerV2>()
            ?? throw new System.Exception(nameof(InGameUIManager) + " cannot find " + nameof(CameraManager));

        mapMgr = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>()
            ?? throw new System.Exception(nameof(InGameUIManager) + " cannot find " + nameof(MapManager));

        mobMgr = GameObject.FindGameObjectWithTag("MobManager").GetComponent<MobManager>()
            ?? throw new System.Exception(nameof(InGameUIManager) + " cannot find " + nameof(MobManager));

        scMgr = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>()
            ?? throw new System.Exception(nameof(InGameUIManager) + " cannot find " + nameof(ScoreManager));

        tbMgr = GameObject.FindGameObjectWithTag("TowerBuildManager").GetComponent<TowerBuildManager>()
            ?? throw new System.Exception(nameof(InGameUIManager) + " cannot find " + nameof(TowerBuildManager));

        ////////////////////////////////////////

        if (buttonPauseGame == null)
            throw new System.Exception(nameof(InGameUIManager) + " cannnot find pause-game-button");
        else
            buttonPauseGame.onClick.AddListener(OnButtonClick_PauseMenu);

        if (buttonChangeToFPSView == null)
            throw new System.Exception(nameof(InGameUIManager) + " cannot find change-view-burron");

        buttonChangeToFPSView.onClick.AddListener(
            delegate
            {
                isFPS = true;
                canvasTopview.SetActive(false);
                canvasFPS.SetActive(true);
            });

        if (pauseMenu == null)
            throw new System.Exception(nameof(InGameUIManager) + " cannot find pause-menu");
        else
        {
            pauseMenu.SetActive(false);

            foreach(Transform buttons in pauseMenu.transform.GetChild(0))
            {
                if(buttons.gameObject.name == "button resume")
                {
                    buttons.GetComponent<Button>().onClick.AddListener(OnButtonClick_Resume);
                }

                if(buttons.gameObject.name == "button restart")
                {
                    buttons.GetComponent<Button>().onClick.AddListener(OnButtonClick_Restart);
                }

                if(buttons.gameObject.name == "button exit")
                {
                    buttons.GetComponent<Button>().onClick.AddListener(OnButtonClick_ExitCurrentGame);
                }
            }
        }

        ///////////////////////////////////////

        if (mapDropdown == null)
            throw new System.Exception("Map list dropdown menu is null");

        if (buttonLoadFromDevice == null)
            throw new System.Exception("Load map from device button is null");

        if (buttonStartGame == null)
            throw new System.Exception("Start game button is null");

        if (buttonSelectToTitle == null)
            throw new System.Exception("Back to Title button is null");

        buttonSelectToTitle.onClick.AddListener(
            delegate
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("SceneTitle");
            });

        if (canvasSelectStage == null)
            throw new System.Exception("Select stage canvas is null");

        if (canvasTopview == null)
            throw new System.Exception("InGame Info canvas is null");

        buttonLoadFromDevice.onClick.AddListener(LoadMapFiles);

        buttonStartGame.onClick.AddListener(OnButtonClick_StartGame);

        mapDropdown.onValueChanged.AddListener(
            delegate
            {
                mapMgr.LoadMapFromDropDownMenu(mapDropdown);
                SetStageInTexts();
            });

        canvasTopview.SetActive(false);
        canvasSelectStage.SetActive(true);

        if (stageNameText == null)
            throw new System.Exception("Stage Name Text (TMP) is null");

        if (stageInfoText == null)
            throw new System.Exception("Stage Info Text (TMP) is null");

        if (stageFlavorText == null)
            throw new System.Exception("Stage Flavor Text (TMP) is null");

        stageNameText.text = "";
        stageInfoText.text = "";
        stageFlavorText.text = "Please Select Stage";

        /////////////////////////////////////////////////

        if (buttonCheatPanel == null)
            throw new System.Exception(nameof(InGameUIManager) + " caannot find reference of " + nameof(buttonCheatPanel));

        if (canvasCheatPanel == null)
            throw new System.Exception(nameof(InGameUIManager) + " caannot find reference of " + nameof(canvasCheatPanel));

        canvasCheatPanel.SetActive(false);

        buttonCheatPanel.onClick.AddListener(
            delegate
            {
                canvasCheatPanel.SetActive(true);
            });


        /////////////////////////////////

        if (canvasFPS == null)
            throw new System.Exception(nameof(InGameUIManager) + " cannot find canvas fps");

        if (buttonChangeToTopview == null)
            throw new System.Exception(nameof(InGameUIManager) + " cannot find change-to-topview-button");

        if (buttonPauseGameinFPS == null)
            throw new System.Exception(nameof(InGameUIManager) + " cannot find pause-game-fps-button");

        canvasFPS.SetActive(false);

        buttonChangeToTopview.onClick.AddListener(
            delegate
            {
                isFPS = false;
                canvasTopview.SetActive(true);
                canvasFPS.SetActive(false);
            });

        buttonPauseGameinFPS.onClick.AddListener(OnButtonClick_PauseMenu);
    }

    private void LoadMapFiles()
    {
        // 디바이스에 저장된 맵 파일을 유저가 새로이 로드하고 싶을 때 누르는 버튼의 스크립트
        // StreamingAssetsPath 는 PC 에서만 동작하니 플랫폼에 따라 맞는 주소값을 사용할 것
        string mapDir = Application.streamingAssetsPath;

        // 드롭다운 메뉴에 사용할 옵션을 저장하는 리스트
        List<TMPro.TMP_Dropdown.OptionData> ddOptions = new List<TMPro.TMP_Dropdown.OptionData>();

        // 특정 디렉토리에서 필터(txt 파일)를 적용한 파일 목록을 읽어옴
        // 반환 형태가 ienumerable<system.io.fileinfo> 이니 어떻게든 받을 수 있게 var 형 변수 선언
        // 이후 foreach 를 이용해 드롭다운 옵션으로 추가하기
        var mapDirList = (new System.IO.DirectoryInfo(mapDir).EnumerateFiles("*.txt"));

        foreach (var data in mapDirList)
            ddOptions.Add(new TMPro.TMP_Dropdown.OptionData(data.Name));

        // TMPro_Dropdown.AddOptions(args) 는 현재 드롭다운의 메뉴의 뒤에 매개변수로 받은 메뉴 목록을 붙여버림
        // 필요에 따라 AddOptins() 혹은 options 프로퍼티를 적당히 골라 사용하자
        mapDropdown.options = ddOptions;
    }

    private void SetStageInTexts()
    {
        string currentStageFull = mapDropdown.options[mapDropdown.value].text;
        string currentStageTrimmed = currentStageFull.Substring(0, currentStageFull.Length - 4);
        string[] trimmed = currentStageTrimmed.Split('_');

        stageNameText.text = currentStageTrimmed;

        string[] flavorTmp = new string[2];

        foreach (string str in trimmed)
        {
            if (str.Contains("by"))
            {
                string[] _trim = str.Split('b');
                stageInfoText.text = _trim[0] + " x " + _trim[1].Substring(1, _trim[1].Length - 1) + " size";
            }

            if (str.Contains("large") || str.Contains("medium") || str.Contains("small"))
            {
                flavorTmp[0] = str;
            }

            if (str.Contains("single") || str.Contains("loop"))
            {
                flavorTmp[1] = str;
            }
        }

        stageFlavorText.text = "Size - " + flavorTmp[0] + "\nType - " + flavorTmp[1];
    }

    private void OnButtonClick_StartGame()
    {
        if (canvasSelectStage.activeSelf)
            canvasSelectStage.SetActive(false);

        if (!canvasTopview.activeSelf)
            canvasTopview.SetActive(true);

        buttonPauseGame.gameObject.SetActive(true);
        scMgr.ResetScore();

        GameObject.FindGameObjectWithTag("MobManager").GetComponent<MobManager>().spawnerEnabled = true;
        GameObject.FindGameObjectWithTag("TowerBuildManager").GetComponentInParent<TowerBuildManager>().ActivateTBM(mapMgr.mapMotherObject.name);
    }

    private void UpdateFlags()
    {
        mobSpawn = GameObject.FindGameObjectWithTag("MobManager").GetComponent<MobManager>().spawnerEnabled;
        userOnSelectMap = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>().mapMotherObject ?? false;
    }

    public void OnButtonClick_PauseMenu()
    {
        userPausedGame = true;
        pauseMenu.SetActive(true);
    }

    public void OnButtonClick_Resume()
    {
        pauseMenu.SetActive(false);
        buttonPauseGame.gameObject.SetActive(true);
        userPausedGame = false;
    }

    public void OnButtonClick_Restart()
    {
        Debug.Log("OnButtonClick_Restart");
        mobMgr.DestroyCurrentSpawnedMobs();
        mapMgr.ResetCurrentMap();
        mapMgr.InstantiateMapCubes();
        pauseMenu.SetActive(false);
        userPausedGame = false;
        tbMgr.buildMenuPanel.SetActive(false);
        tbMgr.enableBuildMenu.gameObject.SetActive(true);
        tbMgr.ActivateTBM(mapMgr.mapMotherObject.name);

        // 맵을 재시작하면 킬 수, 게임머니 등을 초기화할 것
        scMgr.ResetScore();
    }

    public void OnButtonClick_ExitCurrentGame()
    {
        Debug.Log("OnButtonClick_ExitCurrentGame");

        mobMgr.spawnerEnabled = false;
        userPausedGame = false;
        mobMgr.DestroyCurrentSpawnedMobs();
        mapMgr.ResetCurrentMap();
        mapMgr.InstantiateMapCubes();
        canvasSelectStage.SetActive(true);
        canvasTopview.SetActive(false);
        buttonPauseGame.gameObject.SetActive(false);
        pauseMenu.SetActive(false);

        // 맵을 종료하면 킬 수, 게임머니 등을 초기화할 것
        scMgr.ResetScore();
    }

    private void Awake()
    {
        InitializeElements();
        LoadMapFiles();
    }

    private void Start()
    {
        mapMgr.LoadMapFromDropDownMenu(mapDropdown);
        SetStageInTexts();
    }

    private void Update()
    {
        if (userPausedGame)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        UpdateFlags();
    }
}
