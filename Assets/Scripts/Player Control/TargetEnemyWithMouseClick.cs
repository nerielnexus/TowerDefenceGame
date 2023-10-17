using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetEnemyWithMouseClick : MonoBehaviour
{
    private PlayerLockonTarget targetStorage = null;

    private void Awake()
    {
        targetStorage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLockonTarget>();
    }

    private void OnMouseDown()
    {
        GameObject prestoredTarget = targetStorage.GetTargetGameobject();

        if (gameObject != prestoredTarget || prestoredTarget == null)
            targetStorage.SetTarget(gameObject);
        else
            targetStorage.RemoveCurrentTarget();
    }
}
