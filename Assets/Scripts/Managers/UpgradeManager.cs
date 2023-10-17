using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UpgradeManager : MonoBehaviour
{
    private static UpgradeManager instance = null;

    [Header("Upgrade Point")]
    public int upgradePoint_Current = 0;                // 지금 바로 사용 가능한 보유 업그레이드 포인트
    public int upgradePoint_Accumulated = 0;            // 지금까지 획득한 누적 업그레이드 포인트
    public int upgradePoint_BasePrice = 1000;           // 업그레이드 포인트의 기본 구매 비용
    public int upgradePoint_PriceIncreaseStep = 100;    // 업그레이드 포인트를 1점 구매할 때 마다 증가하는 추가 비용

    [Header("Player Tower Upgrade Status")]
    public int towerUpgradeLevel_BaseDamage = 1;        // '기본 대미지 증가' 레벨
    public int towerUpgradeLevel_AttackInterval = 1;    // '공격 속도' 레벨
    public int towerUpgradeLevel_AttackRange = 1;       // '사거리' 레벨
    public int towerUpgradeLevel_ExplodeRadius = 1;     // '폭발 범위' 레벨, 광역딜이 가능한 타워만

    [Header("Tower Upgrade Modifier")]
    public float towerUpgradeModifier_BaseDamage = 1.01f;       // '기본 대미지 증가' 업그레이드의 공격력 증가 배율 (기본 값 x1.01)
    public float towerUpgradeModifier_AttackInterval = 0.5f;   // '공격 속도' 업그레이드의 공격 내부 쿨다운 감소율 (기본 값 x0.99)
    public float towerUpgradeModifier_AttackRange = 1.01f;      // '사거리' 업그레이드의 사거리 증가율 (기본 값 x1.01)
    public float towerUpgradeModifier_ExplodeRadius = 1.01f;     // '폭발 범위' 업그레이드의 범위 증가율 (기본 값 x1.01)

    [Header("Upgrade Text")]
    public List<string> upgradeTextList;

    public static UpgradeManager Instance
    {
        get
        {
            if (instance == null)
            {
                UpgradeManager[] findMgr = FindObjectsOfType<UpgradeManager>();

                if (findMgr.Length == 1)
                {
                    instance = findMgr[0];
                }
                else if (findMgr.Length < 1)        
                {
                    instance = new GameObject("UpgradeManager").AddComponent<UpgradeManager>();
                }

                DontDestroyOnLoad(instance);
            }

            return instance;
        }
        private set { instance = value; }
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            UpgradeManager[] findMgr = FindObjectsOfType<UpgradeManager>();
            if (findMgr.Length != 1)
            {
                Debug.LogWarning("there can be only one game manager");
                Destroy(gameObject);
                return;
            }
        }
    }
}
