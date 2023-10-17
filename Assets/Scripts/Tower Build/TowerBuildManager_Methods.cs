using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class TowerBuildManager : MonoBehaviour
{
    private void InitializeMapCubes()
    {
        GameObject map = (mapObjectName == null) ? null : GameObject.Find(mapObjectName);

        if (map == null) throw new System.Exception("Map is null");

        for(int i=0; i<map.transform.childCount; i++)
        {
            if (map.transform.GetChild(i).name.Equals("Roads"))
                roads = map.transform.GetChild(i).gameObject;
            else if (map.transform.GetChild(i).name.Equals("Build Site"))
                buildSite = map.transform.GetChild(i).gameObject;
        }
        
        for(int i=0; i<buildSite.transform.childCount; i++)
            buildCubeList.Add(buildSite.transform.GetChild(i).gameObject.GetComponent<CubeBuild>());

        foreach(CubeBuild data in buildCubeList)
        {
            data.mgr = this;

            if (data.mgr == null)
                throw new System.Exception("TowerBuildManager is null");
        }
    }

    private void SetUserStateBuildTower()
    {
        if(Input.GetKeyDown(keyBuildTower))
        {
            modeTowerBuild = !modeTowerBuild;

            foreach (CubeBuild data in buildCubeList)
                data.isUserGonnaBuild = modeTowerBuild;
        }
    }

    public GameObject SelectTowerByTowerType()
    {
        return buildableTowerList[(int)currentTypeOfTower] ?? null;
    }

    public TowerAttributes GetAttributeOfSelectedTower()
    {
        return buildableTowerAttributes[(int)currentTypeOfTower] ?? null;
    }

    private void ChangeBuildableTowerType()
    {
        int enumCount = System.Enum.GetNames(typeof(BUILDABLETOWERTYPE)).Length;

        currentTypeOfTower = ((int)currentTypeOfTower < enumCount - 1)
            ? currentTypeOfTower + 1 : 0;
    }

    public void ActivateTBM(string mapName)
    {
        mapObjectName = mapName;

        buildCubeList.Clear();
        // buildCubeList.TrimExcess();

        InitializeMapCubes();

        buildCubeList.TrimExcess();
    }

    private void InitializeElements()
    {
        smgr = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>()
            ?? throw new System.Exception(nameof(TowerBuildManager) + " cannot find " + nameof(ScoreManager));

        if (enableBuildMenu == null)
            throw new System.Exception(nameof(TowerBuildManager) + " has no button element (Enable Build Menu)");
        else
            enableBuildMenu.onClick.AddListener(
                delegate
                {
                    enableBuildMenu.gameObject.SetActive(false);
                    buildMenuPanel.SetActive(true);
                    GameObject.FindGameObjectWithTag("UIManager").GetComponent<InGameUIManager>().buttonChangeToFPSView.gameObject.SetActive(false);

                    modeTowerBuild = true;

                    foreach (CubeBuild data in buildCubeList)
                        data.isUserGonnaBuild = modeTowerBuild;
                });

        if (buildMenuPanel == null)
            throw new System.Exception(nameof(TowerBuildManager) + " has no panel element (Build Menu Panel)");
        else
        {
            buildMenuPanel.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(
                delegate
                {
                    currentTypeOfTower = BUILDABLETOWERTYPE.BIGCANNON;
                    buildMenuPanel.SetActive(false);
                    enableBuildMenu.gameObject.SetActive(true);
                    GameObject.FindGameObjectWithTag("UIManager").GetComponent<InGameUIManager>().buttonChangeToFPSView.gameObject.SetActive(true);

                    modeTowerBuild = false;

                    foreach (CubeBuild data in buildCubeList)
                        data.isUserGonnaBuild = modeTowerBuild;
                });

            buildMenuPanel.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(
                delegate
                {
                    currentTypeOfTower = BUILDABLETOWERTYPE.BIGCANNON;
                });

            buildMenuPanel.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(
                delegate
                {
                    currentTypeOfTower = BUILDABLETOWERTYPE.MUSHROOM;
                });

            buildMenuPanel.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(
                delegate
                {
                    currentTypeOfTower = BUILDABLETOWERTYPE.MACHINEGUN;
                });

            buildMenuPanel.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(
                delegate
                {
                    currentTypeOfTower = BUILDABLETOWERTYPE.CORN;
                });
        }

        buildMenuPanel.SetActive(false);
        enableBuildMenu.gameObject.SetActive(true);
    }

    // ====================================================== unity

    private void Awake()
    {
        InitializeElements();
    }

    private void Update()
    {
        SetUserStateBuildTower();

        if (Input.GetKeyDown(keySelectTowerType))
            ChangeBuildableTowerType();

        buildabletowertypetext.text = currentTypeOfTower.ToString();
        towerBuildModeIndicator.text = "tower build " + modeTowerBuild;
    }
}
