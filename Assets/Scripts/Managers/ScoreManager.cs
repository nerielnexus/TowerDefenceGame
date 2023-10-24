using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Relative Elements")]
    public int killCount = 0;
    public int credit = 0;
    public float creditBoostRatio = 0f;

    [Header("Score UI Elements")]
    public TMPro.TMP_Text[] killCountList = null;
    public TMPro.TMP_Text[] creditList = null;

    public void GainCredit(int baseCredit)
    {
        credit += Mathf.RoundToInt(baseCredit * (1 + creditBoostRatio));
    }

    public void ResetScore()
    {
        killCount = 0;
        GameManager.Instance.currentCredit = GameManager.Instance.initialStartCredit;
    }

    public void SaveCredit()
    {
        killCount = 0;
        GameManager.Instance.currentCredit = credit;
    }

    private void Start()
    {
        if (killCountList == null)
            throw new System.Exception("ScoreManager has no UI elements for kill count");

        if (creditList == null)
            throw new System.Exception("ScoreManager has no UI element for credit");

        credit = GameManager.Instance.currentCredit;
    }

    private void Update()
    {
        if(killCountList.Length > 0)
        {
            foreach (TMPro.TMP_Text kc in killCountList)
                kc.text = killCount.ToString() + " kills";
        }

        if(creditList.Length > 0)
        {
            foreach (TMPro.TMP_Text c in creditList)
                c.text = credit.ToString() + " gold";
        }
    }
}
