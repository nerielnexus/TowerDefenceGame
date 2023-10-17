using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLockonTarget : MonoBehaviour
{
    public Text targetName = null;
    private GameObject target = null;

    public void SetTarget(GameObject target)
    {
        targetName.text = target.name;
        this.target = target;
    }

    public Transform GetTargetTransform()
    {
        return target == null ? null : target.transform;
    }

    public GameObject GetTargetGameobject()
    {
        return target == null ? null : target;
    }

    public void RemoveCurrentTarget()
    {
        targetName.text = "n/a";
        target = null;
    }

    private void Awake()
    {
        targetName.text = target == null ? "n/a" : target.name;
    }
}
