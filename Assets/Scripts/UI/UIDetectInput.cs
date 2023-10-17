using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDetectInput : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        string message = gameObject.name + "/OnClick";
        GameManager.Instance.DetectInputs(message);
        Debug.Log(eventData.pointerCurrentRaycast.gameObject.transform.root.name);
    }
}
