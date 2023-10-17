using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BUILDABLETOWERTYPE
{
    BIGCANNON = 0,
    MUSHROOM,
    MACHINEGUN,
    CORN
};

public partial class TowerBuildManager : MonoBehaviour
{
    [Header("TowerBuildManager Elements")]
    public bool tbmActivated = false;
    public float towerRenderScale = 3f;
    public ScoreManager smgr = null;
    public float refundRatio = 1f;

    [Header("List of cubes on Map")]
    public List<CubeBuild> buildCubeList = new List<CubeBuild>();

    [Header("List of available Towers")]
    public List<GameObject> buildableTowerList = new List<GameObject>();
    public List<string> buildableTowerNameList = new List<string>();
    public List<TowerAttributes> buildableTowerAttributes = new List<TowerAttributes>();

    [Header("Map properties")]
    public string mapObjectName = null;
    public GameObject roads = null;
    public GameObject buildSite = null;

    [Header("Some Key Binds")]
    public KeyCode keyBuildTower = KeyCode.G;
    public bool modeTowerBuild = false;
    public KeyCode keySelectTowerType = KeyCode.H;
    public BUILDABLETOWERTYPE currentTypeOfTower = BUILDABLETOWERTYPE.BIGCANNON;

    [Header("UI Elements")]
    public Button enableBuildMenu = null;
    public GameObject buildMenuPanel = null;

    [Header("TEST")]
    public TMPro.TMP_Text buildabletowertypetext = null;
    public TMPro.TMP_Text towerBuildModeIndicator = null;
}
