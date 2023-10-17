using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour, IPointerClickHandler
{
    /////////////////////////////////////////////////////////////////
    /// <summary>
    /// UI 클릭을 확인하기 위한 IPointerClickHandler 인터페이스 대응부
    /// 타이틀 화면을 클릭했는지 확인하는데 사용함
    /// </summary>

    public void OnPointerClick(PointerEventData eventData)
    {
        string _name = eventData.pointerCurrentRaycast.gameObject.transform.root.name;

        if (_name.Equals(canvas_TitleScreen.name))
        {
            GameManager.Instance.IsGameFreshLaunched = true;
            canvas_TitleScreen.SetActive(false);
            canvas_MainMenu.SetActive(true);
        }
    }

    /////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////
    /// <summary>
    /// Unity 제공 메서드와 공통 변수들
    /// </summary>

    private void Start()
    {
        string _currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        switch(_currentScene)
        {
            case "SceneTitle":
                break;

            case "SceneGamePlay":
                break;

            case "SceneUpgrade":
                break;

            default:
                break;
        }
    }

    /////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////
    /// <summary>
    /// 타이틀 UI
    /// </summary>
    [Header("Title Screen")]
    public GameObject canvas_TitleScreen = null;

    /// <summary>
    /// 게임 타이틀 화면을 불러올 때 구성 요소들을 초기화하는 메서드
    /// </summary>
    private void TitleScreen_Initialize()
    {
        if (canvas_TitleScreen == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(canvas_TitleScreen));
    }
    /////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////
    /// <summary>
    /// 메인메뉴 UI
    /// </summary>
    [Header("Main Menu")]
    public GameObject canvas_MainMenu = null;
    public UnityEngine.UI.Button button_StartGame = null;
    public UnityEngine.UI.Button button_OpenSettings = null;
    public UnityEngine.UI.Button button_OpenCredits = null;
    public UnityEngine.UI.Button button_OpenUpgradeShop = null;

    /// <summary>
    /// 메인메뉴 화면을 불러올 때 구성 요소들을 초기화하는 메서드
    /// </summary>
    private void MainMenu_Initialize()
    {
        // 메인메뉴 캔버스 할당 확인
        if (canvas_MainMenu == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(canvas_MainMenu));

        // 게임시작 버튼 할당 확인
        if (button_StartGame == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(button_StartGame));

        // 설정 버튼 할당 확인
        if (button_OpenSettings == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(button_OpenSettings));


        // 만든이들 버튼 할당 확인
        if (button_OpenCredits == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(button_OpenCredits));

        // 메인 메뉴 -> 업그레이드 상점의 경우에 사용할 할당 및 기능 구현
        // 업그레이드 상점 버튼 할당 확인
        if (button_OpenUpgradeShop == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(button_OpenUpgradeShop));
    }

    /// <summary>
    /// 메인 메뉴 버튼들의 기능 할당
    /// </summary>
    private void MainMenu_AddListener()
    {
        // 게임시작 버튼의 클릭 시 기능 할당
        button_StartGame.onClick.AddListener(
            delegate
            {
                // Scene Trace 관리
                GameManager.Instance.sceneTrace_before = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                GameManager.Instance.sceneTrace_after = "SceneGamePlay";

                // 게임플레이 씬 불러오기
                UnityEngine.SceneManagement.SceneManager.LoadScene("SceneGamePlay");
            });

        // 설정 버튼의 클릭 시 기능 할당
        button_OpenSettings.onClick.AddListener(
            delegate
            {
                // 각 버튼들을 비활성화해서 클릭 방지 및 숨겨주기
                button_StartGame.gameObject.SetActive(false);
                button_OpenSettings.gameObject.SetActive(false);
                button_OpenCredits.gameObject.SetActive(false);
                button_OpenUpgradeShop.gameObject.SetActive(false);

                // 설정화면 캔버스 활성화해서 보여주기
                canvas_Settings.SetActive(true);
            });

        // 만든이들 버튼 클릭 시 기능 할당
        button_OpenCredits.onClick.AddListener(
            delegate
            {
                // 각 버튼들을 비활성화해서 클릭 방지 및 숨겨주기
                button_StartGame.gameObject.SetActive(false);
                button_OpenSettings.gameObject.SetActive(false);
                button_OpenCredits.gameObject.SetActive(false);
                button_OpenUpgradeShop.gameObject.SetActive(false);

                // 만든이들 캔버스 활성화해서 보여주기
                canvas_Credits.gameObject.SetActive(true);
            });

        // 업그레이드 상점 버튼 클릭 시 기능 할당
        button_OpenUpgradeShop.onClick.AddListener(
            delegate
            {
                // Scene Trace 관리
                GameManager.Instance.sceneTrace_before = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                GameManager.Instance.sceneTrace_after = "SceneUpgrade";

                // 업그레이드 씬 불러오기
                UnityEngine.SceneManagement.SceneManager.LoadScene("SceneUpgrade");
            });
    }
    /////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////
    /// <summary>
    /// 설정 UI
    /// </summary>
    [Header("Settings")]
    public GameObject canvas_Settings = null;
    public TMPro.TMP_InputField inputfield_CustonInitialCredit = null;
    public UnityEngine.UI.Button button_ConfirmCredit = null;
    public UnityEngine.UI.Button button_LeaveSettings = null;

    /// <summary>
    /// 설정 화면을 불러올 때 구성 요소들을 초기화하는 메서드
    /// </summary>
    private void Settings_Initialize()
    {
        // 설정 화면 캔버스 할당 확인
        if (canvas_Settings == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(canvas_Settings));

        // 시작 크레딧을 변경할 때 사용하는 텍스트 입력 공간 할당 확인
        if (inputfield_CustonInitialCredit == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(inputfield_CustonInitialCredit));

        // 시작 크레딧을 변경할 때 사용하는 확인 버튼 할당 확인
        if (button_ConfirmCredit == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(button_ConfirmCredit));

        // 설정 화면에서 나가는 버튼 할당 확인
        if (button_LeaveSettings == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(button_LeaveSettings));
    }

    /// <summary>
    /// 설정 버튼들의 기능 할당
    /// </summary>
    private void Settings_AddListner()
    {
        // 설정 화면에서 나가는 버튼의 기능 할당
        button_LeaveSettings.onClick.AddListener(
            delegate
            {
                string _nextScene = GameManager.Instance.sceneTrace_before;

                // Scene Trace 관리
                GameManager.Instance.sceneTrace_before = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                GameManager.Instance.sceneTrace_after = "SceneTitle";

                // 타이틀 씬으로 넘겨주기
                UnityEngine.SceneManagement.SceneManager.LoadScene("SceneTitle");
            });
    }
    /////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////
    /// <summary>
    /// 만든이들 UI
    /// </summary>
    [Header("Credits")]
    public GameObject canvas_Credits = null;
    public TMPro.TMP_Text text_Credits = null;
    public UnityEngine.UI.Button button_LeaveCredits = null;

    /// <summary>
    /// 만든이들 화면을 불러올 때 구성 요소들을 초기화하는 메서드
    /// </summary>
    private void Credits_Initialize()
    {
        // 만든이들 캔버스 할당 확인
        if (canvas_Credits == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(canvas_Credits));

        // 만든이들 텍스트 할당 확인
        if (text_Credits == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(text_Credits));

        // 만든이들 화면에서 나갈 버튼 할당 확인
        if (button_LeaveCredits == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(button_LeaveCredits));
    }

    /// <summary>
    /// 만든이들 버튼들의 기능 할당
    /// </summary>
    private void Credits_AddListner()
    {
        // 만든이들 화면에서 메인 화면으로 돌아갈 버튼 기능 할당
        button_LeaveCredits.onClick.AddListener(
            delegate
            {
                // 타이틀 씬 불러오기
                UnityEngine.SceneManagement.SceneManager.LoadScene("SceneTitle");
            });
    }
    /////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////
    /// <summary>
    /// 업그레이드 상점 UI
    /// </summary>
    [Header("Upgrade Shop")]
    public UpgradeManager instance_UpgradeManager = null;
    public GameObject canvas_UpgradeShop = null;
    public TMPro.TMP_Dropdown dropdown_UpgradeSelectionMenu = null;
    public UnityEngine.UI.Button button_BuyUpgradePoint = null;
    public UnityEngine.UI.Button button_ConfirmUpgrade = null;
    public UnityEngine.UI.Button button_RevertUpgrade = null;
    public UnityEngine.UI.Button button_LeaveUpgradeShop = null;
    public TMPro.TMP_Text text_UpgradePointPrice = null;
    public TMPro.TMP_Text text_UpgradePointSpent = null;
    public TMPro.TMP_Text text_UpgradeFlavorText = null;
    public TMPro.TMP_Text text_CurrentCredit = null;
    public TMPro.TMP_Text text_CurrentUpgradePoint = null;
    public int currentUpgradePointPrice = 0;

    /// <summary>
    /// 업그레이드 상점 화면을 불러올 때 구성 요소들을 초기화하는 메서드
    /// </summary>
    private void UpgradeShop_Initialize()
    {
        // 업그레이드 매니저 인스턴스 할당 확인
        if (instance_UpgradeManager == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(instance_UpgradeManager));

        // 업그레이드 상점 캔버스 할당 확인
        if (canvas_UpgradeShop == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(canvas_UpgradeShop));

        // 업그레이드 메뉴를 선택할 드롭다운 메뉴 할당 확인
        if (dropdown_UpgradeSelectionMenu == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(dropdown_UpgradeSelectionMenu));

        // 업그레이드 포인트 구매 버튼 할당 확인
        if (button_BuyUpgradePoint == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(button_BuyUpgradePoint));

        // 업그레이드 포인트를 소모하는 버튼 할당 확인
        if (button_ConfirmUpgrade == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(button_ConfirmUpgrade));

        // 업그레이드 포인트 소모 내역을 초기화하는 버튼 할당 확인
        if (button_RevertUpgrade == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(button_RevertUpgrade));

        // 업그레이드 상점을 떠나는 버튼 할당 확인
        if (button_LeaveUpgradeShop == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(button_LeaveUpgradeShop));

        // 업그레이드 포인트 가격을 표시할 텍스트 할당 확인
        if (text_UpgradePointPrice == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(text_UpgradePointPrice));

        // 사용한 누적 업그레이드 포인트를 표시할 텍스트 할당 확인
        if (text_UpgradePointSpent == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(text_UpgradePointSpent));

        // 업그레이드 설명을 띄울 텍스트 할당 확인
        if (text_UpgradeFlavorText == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(text_UpgradeFlavorText));

        // 현재 보유중인 크레딧을 띄울 텍스트 할당 확인
        if (text_CurrentCredit == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(text_CurrentCredit));

        // 현재 보유중인 업그레이드 포인트를 띄울 텍스트 할당 확인
        if (text_CurrentUpgradePoint == null)
            throw new System.Exception(nameof(UIManager) + " - failed to reference " + nameof(text_CurrentUpgradePoint));
    }
    /////////////////////////////////////////////////////////////////
}
