using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIManager : MonoBehaviour
{
    [Header("Canvas Elements")]
    public GameObject canvasTitle = null;
    public GameObject canvasMainMenu = null;
    public GameObject canvasOption = null;
    public GameObject canvasCredit = null;
    public GameObject canvasCheat = null;

    // 게임 첫 실행 확인용, 그래야 타이틀 화면을 덜 보여줄테니
    [Header("Title Screen Elements")]
    public bool gameInitialStart = false;

    [Header("Main Menu Elements")]
    public UnityEngine.UI.Button buttonStartGame = null;
    public UnityEngine.UI.Button buttonOption = null;
    public UnityEngine.UI.Button buttonCredit = null;

    [Header("Option Elements")]
    public UnityEngine.UI.Button buttonOpenSettings = null;
    public UnityEngine.UI.Button buttonSaveSettingsChange = null;
    public UnityEngine.UI.Button buttonRevertSettingChange = null;
    public UnityEngine.UI.Button buttonOpenCheatPanel = null;

    // 시쳇말로 '만든놈들' 화면용 변수들
    [Header("Credit Elements")]
    public TMPro.TMP_Text creditText = null;
    public UnityEngine.UI.Button buttonCreditToMain = null;

    // 업그레이드 상점 씬을 여는 버튼
    [Header("Upgrade UI Elements")]
    public UnityEngine.UI.Button buttonUpgradeScene = null;

    private void InitializeUIElements()
    {
        if (canvasTitle == null)
            throw new System.Exception(nameof(GameManager) + " - title canvas is null");

        if (canvasMainMenu == null)
            throw new System.Exception(nameof(GameManager) + " - main menu canvas is null");

        if (canvasOption == null)
            throw new System.Exception(nameof(GameManager) + " - option canvas is null");

        if (canvasCredit == null)
            throw new System.Exception(nameof(GameManager) + " - credit canvas is null");

        if (buttonStartGame == null)
            throw new System.Exception(nameof(GameManager) + " - start game button is null");

        if (buttonUpgradeScene == null)
            throw new System.Exception(nameof(GameManager) + " - upgrade scene button is null");

        buttonStartGame.onClick.AddListener(
            delegate
            {
                buttonStartGame.gameObject.SetActive(false);
                buttonOption.gameObject.SetActive(false);
                buttonCredit.gameObject.SetActive(false);
                buttonUpgradeScene.gameObject.SetActive(false);

                UnityEngine.SceneManagement.SceneManager.LoadScene("SceneGamePlay");
            });

        if (buttonOption == null)
            throw new System.Exception(nameof(GameManager) + " - option button is null");

        buttonOption.onClick.AddListener(
            delegate
            {
                buttonStartGame.gameObject.SetActive(false);
                buttonOption.gameObject.SetActive(false);
                buttonCredit.gameObject.SetActive(false);
                buttonUpgradeScene.gameObject.SetActive(false);

                canvasOption.SetActive(true);
            });

        if (buttonCredit == null)
            throw new System.Exception(nameof(GameManager) + " - credit button is null");

        buttonCredit.onClick.AddListener(
            delegate
            {
                buttonStartGame.gameObject.SetActive(false);
                buttonOption.gameObject.SetActive(false);
                buttonCredit.gameObject.SetActive(false);
                buttonUpgradeScene.gameObject.SetActive(false);

                canvasCredit.SetActive(true);
            });

        // 설정 settings 관련
        if (buttonOpenSettings == null)
            throw new System.Exception(nameof(TitleUIManager) + " cannot find reference of " + nameof(buttonOpenSettings));

        if (buttonSaveSettingsChange == null)
            throw new System.Exception(nameof(TitleUIManager) + " cannot find reference of " + nameof(buttonSaveSettingsChange));

        if (buttonRevertSettingChange == null)
            throw new System.Exception(nameof(TitleUIManager) + " cannot find reference of " + nameof(buttonRevertSettingChange));

        if (buttonOpenCheatPanel == null)
            throw new System.Exception(nameof(TitleUIManager) + " cannot find reference of " + nameof(buttonOpenCheatPanel));

        if(canvasCheat == null)
            throw new System.Exception(nameof(TitleUIManager) + " cannot find reference of " + nameof(canvasCheat));

        buttonOpenSettings.onClick.AddListener(
            delegate
            {
                canvasOption.SetActive(true);
            });

        buttonSaveSettingsChange.onClick.AddListener(
            delegate
            {
                Debug.Log("Setting has changed successfully");
                canvasOption.SetActive(false);

                buttonStartGame.gameObject.SetActive(true);
                buttonOption.gameObject.SetActive(true);
                buttonCredit.gameObject.SetActive(true);
                buttonUpgradeScene.gameObject.SetActive(true);
            });

        buttonRevertSettingChange.onClick.AddListener(
            delegate
            {
                Debug.Log("Unsaved change of settings has deleted");
                canvasOption.SetActive(false);

                buttonStartGame.gameObject.SetActive(true);
                buttonOption.gameObject.SetActive(true);
                buttonCredit.gameObject.SetActive(true);
                buttonUpgradeScene.gameObject.SetActive(true);
            });

        buttonOpenCheatPanel.onClick.AddListener(
            delegate
            {
                canvasCheat.SetActive(true);
            });



        if (creditText == null)
            throw new System.Exception(nameof(GameManager) + " - credit text is null");


        SetCreditText();

        if (buttonCreditToMain == null)
            throw new System.Exception(nameof(GameManager) + " - credit to main button is null");

        buttonCreditToMain.onClick.AddListener(
            delegate
            {
                canvasCredit.SetActive(false);

                buttonStartGame.gameObject.SetActive(true);
                buttonOption.gameObject.SetActive(true);
                buttonCredit.gameObject.SetActive(true);
                buttonUpgradeScene.gameObject.SetActive(true);
            });

        buttonUpgradeScene.onClick.AddListener(
            delegate
            {
                SceneManager.LoadScene("SceneUpgrade");
            });
    }

    [System.Obsolete("Follow instruction in commentary section to improve method in later time",false)]
    private void SetCreditText()
    {
        // 나중에 크레딧을
        // (1) 외부에서 텍스트 파일을 불러와서
        // (2) 적절하게 화면에 뿌려주는 형태
        // 로 새로이 만들것
        creditText.text = "만든 이 - GJJ";

    }

    public void LoadCanvas()
    {
        if(gameInitialStart)
        {
            canvasTitle.SetActive(false);
            canvasMainMenu.SetActive(true);
            canvasOption.SetActive(false);
            canvasCredit.SetActive(false);
        }
        else
        {
            canvasTitle.SetActive(true);
            canvasMainMenu.SetActive(false);
            canvasOption.SetActive(false);
            canvasCredit.SetActive(false);
        }
    }

    private void Awake()
    {
        InitializeUIElements();
    }
}
