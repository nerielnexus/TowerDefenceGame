using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////////////////////////////////////////////////////
///
/// SQLite DB 영역
/// 
/// DB 영역은 추후 서버쪽으로 옮겨야 함
/// 지금 (작성시점 23.08.23) 은 기능 구현 및 테스트를 위해 GameManager 에 연동함
/// 
/////////////////////////////////////////////////////////////////////////////////////////

/*
using Mono.Data.Sqlite;
using System.Data;
*/

/////////////////////////////////////////////////////////////////////////////////////////

public class GameManager : MonoBehaviour
{
    //==============================
    /// <summary>
    /// 게임 매니저를 싱글톤으로 구성하기
    /// </summary>

    private static GameManager instance = null;

    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                GameManager[] findGM = FindObjectsOfType<GameManager>();

                if (findGM.Length == 1)
                {
                    instance = findGM[0];
                }
                else if (findGM.Length < 1)
                {
                    instance = new GameObject("GameManager").AddComponent<GameManager>();
                }

                DontDestroyOnLoad(instance);
            }

            return instance;
        }
        private set { instance = value; }
    }

    //==============================

    [Header("Other Scene-Relative Managers")]
    public GameObject managerObject = null;

    [Header("Elements relate with other Managers")]
    public int currentCredit = 0;
    public int userTypedCredit = 0;
    public bool gameInitialStarted = false;

    [Header("Entity Specs")]
    public float towerTriggerRange = 7f;
    public float towerTriggerInterval = 0.1f;
    public float mobSpawnInterval = 0.8f;

    [Header("Booleans for check initial launch")]
    public bool IsGameFreshLaunched = false;

    [Header("Scene 이동 추적을 위한 텍스트")]
    public string sceneTrace_before = null;
    public string sceneTrace_after = null;

    //==============================

    public void DetectInputs(string _message)
    {
        TitleUIManager tuim = managerObject.GetComponent<TitleUIManager>();

        if(_message.Equals(tuim.canvasTitle.name + "/OnClick"))
        {
            gameInitialStarted = true;
            tuim.gameInitialStart = true;
            tuim.canvasTitle.SetActive(false);
            tuim.canvasMainMenu.SetActive(true);
        }
    }

    public void CheckWhichSceneIsLoaded()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        switch(currentSceneName)
        {
            case "SceneTitle":
                managerObject = GameObject.Find("Title UI Manager")
                    ?? throw new System.Exception(nameof(GameManager) + "cannot find " + nameof(TitleUIManager) + " object");

                managerObject.GetComponent<TitleUIManager>().gameInitialStart = gameInitialStarted;
                managerObject.GetComponent<TitleUIManager>().LoadCanvas();
                break;

            case "SceneGamePlay":
                if (!GameObject.Find("MapManager"))
                    throw new System.Exception(nameof(GameManager) + "cannot find " + nameof(MapManager) + " object");

                if (!GameObject.Find("MobSpawnManager"))
                    throw new System.Exception(nameof(GameManager) + "cannot find " + nameof(MobManager) + " object");

                if (!GameObject.Find("TowerBuildManager"))
                    throw new System.Exception(nameof(GameManager) + "cannot find " + nameof(TowerBuildManager) + " object");

                if (!GameObject.Find("ScoreManager"))
                    throw new System.Exception(nameof(GameManager) + "cannot find " + nameof(ScoreManager) + " object");

                if (!GameObject.Find("UIManager"))
                    throw new System.Exception(nameof(GameManager) + "cannot find " + nameof(InGameUIManager) + " object");

                break;

            case "SceneUpgrade":
                break;
        }
    }

    //==============================

    private void Awake()
    {
        if(!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += delegate { CheckWhichSceneIsLoaded(); };
        }
        else
        {
            GameManager[] findGM = FindObjectsOfType<GameManager>();
            if (findGM.Length != 1)
            {
                Debug.LogWarning("there can be only one game manager");
                Destroy(gameObject);
                return;
            }
        }

        InitializeSQLiteDB();
    }

    private void OnDestroy()
    {
        instance = null;
    }


    /////////////////////////////////////////////////////////////////////////////////////////
    ///
    /// SQLite DB 영역
    /// 
    /// DB 영역은 추후 서버쪽으로 옮겨야 함
    /// 지금 (작성시점 23.08.23) 은 기능 구현 및 테스트를 위해 GameManager 에 연동함
    /// 
    /// 코드 작성 시 주의할 점
    /// 
    /// 1. 다른 Manager 에서 접근할 변수, 메서드는 public 으로 선언할 것
    /// 2. 실제로 DB 에 접근을 하는, 서버 쪽에서 동작해야 하는 변수와 메서드는 private 로 선언할 것
    /// 3. 아몰랑캡슐화잘해
    /// 
    /////////////////////////////////////////////////////////////////////////////////////////

    private bool isDBTest = true;                               // db 를 테스트 용도로 쓰고 있는지? (23.08.23 기준 true)
    private string dbFileAddress = null;                        // db 의 파일이 저장된 경로
    private string dbFileName = null;                           // db 파일 이름
    private string sqlQuery = null;                             // db 쿼리문 스트링

    private System.Data.IDbConnection dbEntrypoint = null;      // unity 에서 db 에 접근할 때 사용할 객체
    private System.Data.IDbCommand dbCommandString = null;      // unity 에서 db 에 쿼리를 보낼 때 사용할 객체

    /*
     * SqliteConnection 과 IDbConnection
     * 
     * SqliteConnection 은 DbConnection 클래스를 상속하며 DbConnection 은 IDbConnection 인터페이스를 따르는 클래스
     * 대부분의 SQLite 예시가 SqliteConnection 객체를 IDbConnection 으로 캐스팅해서 사용하는데,
     * 어차피 SQLite 를 확실하게 쓸거라면 그렇게 작성할 필요가 있나? 라는 생각이 든다
     * 
     * 하지만 나중에 서버쪽으로 옮기고, 다른 DBMS 를 사용하게 된다면 예시대로 IDbConnection 변수를 쓰는게 맞을지도
     * 
     * 나중에 서버로 옮길 때 생각하자
     */


    private void SetDatabaseName()
    {
        if(isDBTest)
        {
            dbFileName = "userdatabase_TEST.s3db";
        }
        else
        {
            dbFileName = "userdatabase.s3db";
        }

        dbFileAddress = Application.dataPath + "/" + dbFileName;
    }

    private void InitializeSQLiteDB()
    {
        SetDatabaseName();

        if(!System.IO.File.Exists(dbFileName))
        {
            Mono.Data.Sqlite.SqliteConnection.CreateFile(dbFileAddress);
        }

        
        try
        {
            dbEntrypoint = new Mono.Data.Sqlite.SqliteConnection("URI=file:" + dbFileAddress);
            Debug.Log("SQLite open DB in " + dbFileAddress);
            dbEntrypoint.Open();
        }
        catch(System.Exception e)
        {
            Debug.Log("SQLite open DB failed in " + dbFileAddress);
            dbEntrypoint.Close();
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////
}
