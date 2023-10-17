using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 추후 타워에 업그레이드 성능 반영할 것. 지금은 공속만 해둠

public class UpgradeModule : MonoBehaviour
{
    [Header("Upgrade UI Entity")]
    public TMPro.TMP_Dropdown upgradeUI_DropdownMenu = null;          // 업그레이드를 선택할 드롭다운 메뉴
    public UnityEngine.UI.Button upgradeUI_BuyUpgradePoint = null;    // 업그레이드 포인트를 구매하는 버튼
    public UnityEngine.UI.Button upgradeUI_SaveUpgrade = null;        // 플레이어의 업그레이드 포인트 사용 내역을 저장하는 버튼
    public UnityEngine.UI.Button upgradeUI_RevertUpgrade = null;      // 플레이어의 업그레이드 포인트 사용 내역을 초기화하는 버튼
    public UnityEngine.UI.Button upgradeUI_BackToMenu = null;         // 플레이어가 업그레이드 메뉴에서 메인 메뉴로 돌아가는 버튼

    [Header("Upgrade UI Text")]
    public TMPro.TMP_Text upgradeText_Price = null;                 // 플레이어가 포인트를 구매할 때 지불할 크레딧을 표시하는 텍스트
    public TMPro.TMP_Text upgradeText_UsedPoint = null;             // 플레이어가 해당 업그레이드에 사용한 포인트를 표시하는 텍스트
    public TMPro.TMP_Text upgradeText_FlavorText = null;            // 해당 업그레이드에 대한 설명을 표시하는 텍스트
    public TMPro.TMP_Text upgradeText_CurrentCredit = null;         // 플레이어가 지금 보유중인 크레딧을 표시하는 텍스트
    public TMPro.TMP_Text upgradeText_CurrentPoint = null;          // 플레이어가 지금 보유중인 포인트를 표시하는 텍스트

    public int currentPrice = 0;

    public void UpgradeWithSelected()
    {
        if (upgradeUI_DropdownMenu.captionText.text.Equals("BaseDamage"))
        {
            upgradeText_UsedPoint.text = UpgradeManager.Instance.towerUpgradeLevel_BaseDamage.ToString() + " 포인트 사용함";

            float _valueBefore = (UpgradeManager.Instance.towerUpgradeModifier_BaseDamage - 1f) * 100f * UpgradeManager.Instance.towerUpgradeLevel_BaseDamage;
            float _valueAfter = (UpgradeManager.Instance.towerUpgradeModifier_BaseDamage - 1f) * 100f * (UpgradeManager.Instance.towerUpgradeLevel_BaseDamage + 1);
            upgradeText_FlavorText.text = "포탑의 대미지가 " + System.Math.Round(_valueBefore, 0) + "% 만큼 증가합니다.\n"
                + "다음 단계에서는 " + System.Math.Round(_valueAfter, 0) + "% 만큼 증가합니다.";
        }

        if (upgradeUI_DropdownMenu.captionText.text.Equals("AttackInterval"))
        {
            upgradeText_UsedPoint.text = UpgradeManager.Instance.towerUpgradeLevel_AttackInterval.ToString() + " 포인트 사용함";

            float _valueBefore = UpgradeManager.Instance.towerUpgradeModifier_AttackInterval * 100f * UpgradeManager.Instance.towerUpgradeLevel_AttackInterval;
            float _valueAfter = UpgradeManager.Instance.towerUpgradeModifier_AttackInterval * 100f * (UpgradeManager.Instance.towerUpgradeLevel_AttackInterval + 1);
            upgradeText_FlavorText.text = "포탑의 공격 속도가 " + System.Math.Round(_valueBefore, 0) + "% 만큼 증가합니다.\n"
                + "다음 단계에서는 " + System.Math.Round(_valueAfter, 0) + "% 만큼 증가합니다.";
        }

        if (upgradeUI_DropdownMenu.captionText.text.Equals("AttackRange"))
        {
            upgradeText_UsedPoint.text = UpgradeManager.Instance.towerUpgradeLevel_AttackRange.ToString() + " 포인트 사용함";

            float _valueBefore = (UpgradeManager.Instance.towerUpgradeModifier_AttackRange - 1f) * 100f * UpgradeManager.Instance.towerUpgradeLevel_AttackRange;
            float _valueAfter = (UpgradeManager.Instance.towerUpgradeModifier_AttackRange - 1f) * 100f * (UpgradeManager.Instance.towerUpgradeLevel_AttackRange + 1);
            upgradeText_FlavorText.text = "포탑의 사거리가 " + System.Math.Round(_valueBefore, 0) + "% 만큼 증가합니다.\n"
                + "다음 단계에서는 " + System.Math.Round(_valueAfter, 0) + "% 만큼 증가합니다.";
        }

        if (upgradeUI_DropdownMenu.captionText.text.Equals("ExplodeRadius"))
        {
            upgradeText_UsedPoint.text = UpgradeManager.Instance.towerUpgradeLevel_ExplodeRadius.ToString() + " 포인트 사용함";

            float _valueBefore = (UpgradeManager.Instance.towerUpgradeModifier_ExplodeRadius - 1f) * 100f * UpgradeManager.Instance.towerUpgradeLevel_ExplodeRadius;
            float _valueAfter = (UpgradeManager.Instance.towerUpgradeModifier_ExplodeRadius - 1f) * 100f * (UpgradeManager.Instance.towerUpgradeLevel_ExplodeRadius + 1);
            upgradeText_FlavorText.text = "폭발 범위가 " + System.Math.Round(_valueBefore, 0) + "% 만큼 증가합니다.\n"
                + "다음 단계에서는 " + System.Math.Round(_valueAfter, 0) + "% 만큼 증가합니다.";
        }
    }

    public void UpgradeSelectedOption()
    {
        if(UpgradeManager.Instance.upgradePoint_Current <= 0)
        {
            Debug.Log("cannot do upgrade : upgrade point is 0 or below");
            return;
        }

        if (upgradeUI_DropdownMenu.captionText.text.Equals("BaseDamage"))
        {
            UpgradeManager.Instance.towerUpgradeLevel_BaseDamage++;
            UpgradeManager.Instance.upgradePoint_Current--;
        }

        if (upgradeUI_DropdownMenu.captionText.text.Equals("AttackInterval"))
        {
            UpgradeManager.Instance.towerUpgradeLevel_AttackInterval++;
            UpgradeManager.Instance.upgradePoint_Current--;
        }

        if (upgradeUI_DropdownMenu.captionText.text.Equals("AttackRange"))
        {
            UpgradeManager.Instance.towerUpgradeLevel_AttackRange++;
            UpgradeManager.Instance.upgradePoint_Current--;
        }

        if (upgradeUI_DropdownMenu.captionText.text.Equals("ExplodeRadius"))
        {
            UpgradeManager.Instance.towerUpgradeLevel_ExplodeRadius++;
            UpgradeManager.Instance.upgradePoint_Current--;
        }

        UpgradeWithSelected();
    }

    public void BuyUpgradePoint()
    {
        if(GameManager.Instance.currentCredit - currentPrice < 0)
        {
            Debug.Log("cannot do upgrade : insufficient credit");
        }
        else
        {
            GameManager.Instance.currentCredit -= currentPrice;
            UpgradeManager.Instance.upgradePoint_Current++;
            UpgradeManager.Instance.upgradePoint_Accumulated++;
            currentPrice += UpgradeManager.Instance.upgradePoint_PriceIncreaseStep;
        }
    }

    public void ResetUpgradePoint()
    {
        UpgradeManager.Instance.upgradePoint_Current = UpgradeManager.Instance.upgradePoint_Accumulated;
        UpgradeManager.Instance.towerUpgradeLevel_BaseDamage = 0;
        UpgradeManager.Instance.towerUpgradeLevel_AttackRange = 0;
        UpgradeManager.Instance.towerUpgradeLevel_AttackInterval = 0;
        UpgradeManager.Instance.towerUpgradeLevel_ExplodeRadius = 0;

        UpgradeWithSelected();
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (upgradeUI_BackToMenu == null)
            throw new System.Exception(nameof(UpgradeModule) + " - back to menu button is null");

        upgradeUI_BackToMenu.onClick.AddListener(
            delegate
            {
                SceneManager.LoadScene("SceneTitle");
            });

        if (upgradeUI_DropdownMenu == null)
            throw new System.Exception(nameof(UpgradeModule) + " - dropdown menu is null");

        upgradeUI_DropdownMenu.ClearOptions();
        upgradeUI_DropdownMenu.AddOptions(UpgradeManager.Instance.upgradeTextList);

        upgradeUI_DropdownMenu.onValueChanged.AddListener(
            delegate
            {
                UpgradeWithSelected();
            });

        if (upgradeUI_DropdownMenu.captionText.text.Equals("BaseDamage"))
        {
            upgradeText_UsedPoint.text = UpgradeManager.Instance.towerUpgradeLevel_BaseDamage.ToString() + " 포인트 사용함";

            float _valueBefore = (UpgradeManager.Instance.towerUpgradeModifier_BaseDamage - 1f) * 100f * UpgradeManager.Instance.towerUpgradeLevel_BaseDamage;
            float _valueAfter = (UpgradeManager.Instance.towerUpgradeModifier_BaseDamage - 1f) * 100f * (UpgradeManager.Instance.towerUpgradeLevel_BaseDamage + 1);
            upgradeText_FlavorText.text = "포탑의 대미지가 " + System.Math.Round(_valueBefore, 0) + "% 만큼 증가합니다.\n"
                + "다음 단계에서는 " + System.Math.Round(_valueAfter, 0) + "% 만큼 증가합니다.";
        }

        if (upgradeUI_SaveUpgrade == null)
            throw new System.Exception(nameof(UpgradeModule) + " - save upgrade button is null:");

        upgradeUI_SaveUpgrade.onClick.AddListener(
            delegate
            {
                UpgradeSelectedOption();
            });

        if (upgradeUI_BuyUpgradePoint == null)
            throw new System.Exception(nameof(UpgradeModule) + " - buy upgrade button is null");

        upgradeUI_BuyUpgradePoint.onClick.AddListener(
            delegate
            {
                BuyUpgradePoint();
            });

        if (upgradeUI_RevertUpgrade == null)
            throw new System.Exception(nameof(UpgradeModule) + " - revert upgrade button is null");

        upgradeUI_RevertUpgrade.onClick.AddListener(
            delegate
            {
                ResetUpgradePoint();
            });

        currentPrice = UpgradeManager.Instance.upgradePoint_BasePrice;
    }

    // Update is called once per frame
    void Update()
    {
        upgradeText_CurrentCredit.text = GameManager.Instance.currentCredit.ToString();
        upgradeText_CurrentPoint.text = UpgradeManager.Instance.upgradePoint_Current.ToString();
        upgradeText_Price.text = currentPrice.ToString();
    }
}
