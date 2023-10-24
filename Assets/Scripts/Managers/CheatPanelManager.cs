using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatPanelManager : MonoBehaviour
{
    [Header("Elements - Modify Popup")]
    public GameObject panel_ModifyPopup = null;
    public TMPro.TMP_InputField inputfield_TypeToModifyCurrentCredit = null;
    public TMPro.TMP_Text text_CurrentCredit = null;
    public UnityEngine.UI.Button button_SaveCurrentCredit = null;
    
    [Header("Elements - Result Popup")]
    public GameObject panel_ResultPopup = null;
    public TMPro.TMP_Text resultText = null;

    [Header("Elements - ETC")]
    public int tempCredit = 0;
    public bool inActiveCalled = false;
    public float inactiveTimer = 0f;
    public float inactiveDelay = 2f;

    [Header("Available Status Image (O X -)")]
    public UnityEngine.UI.Image[] image_LegitTypedStatus = null;

    private void SetStatusImage(int index)
    {
        if(index >= image_LegitTypedStatus.Length)
        {
            throw new System.Exception(nameof(CheatPanelManager) + " : " + nameof(SetStatusImage) + "-  index is out or range");
        }

        for(int i=0; i<image_LegitTypedStatus.Length; i++)
        {
            image_LegitTypedStatus[i].gameObject.SetActive(false);
        }

        image_LegitTypedStatus[index].gameObject.SetActive(true);
    }

    private void SetCurrentCreditButton()
    {
        GameManager.Instance.currentCredit = tempCredit;

        if(tempCredit != 0)
        {
            resultText.text = "Current credit has modified by User \n\n" + GameManager.Instance.currentCredit.ToString();
        }
        else
        {
            resultText.text = "Current credit not changed";
        }

        panel_ModifyPopup.SetActive(false);
        panel_ResultPopup.SetActive(true);

        inActiveCalled = true;
    }

    private void Awake()
    {
        panel_ModifyPopup.SetActive(true);
        panel_ResultPopup.SetActive(false);

        inputfield_TypeToModifyCurrentCredit.onValueChanged.AddListener(
            delegate
            {
                int resultCredit = 0;

                if (int.TryParse(inputfield_TypeToModifyCurrentCredit.text, out resultCredit))
                {
                    tempCredit = resultCredit;
                    SetStatusImage(0);
                }
                else
                {
                    tempCredit = 0;
                    SetStatusImage(1);
                }

                if(inputfield_TypeToModifyCurrentCredit.text == "")
                {
                    tempCredit = 0;
                    SetStatusImage(2);
                }
            });

        button_SaveCurrentCredit.onClick.AddListener(SetCurrentCreditButton);
    }

    private void Update()
    {
        text_CurrentCredit.text = GameManager.Instance.currentCredit.ToString();

        if(inActiveCalled)
        {
            if(inactiveTimer < inactiveDelay)
                inactiveTimer += Time.deltaTime;
            else
            {
                inActiveCalled = false;
                panel_ModifyPopup.SetActive(true);
                panel_ResultPopup.SetActive(false);
                inputfield_TypeToModifyCurrentCredit.text = "";
                gameObject.SetActive(false);
            }
        }
    }
}
