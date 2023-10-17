using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TowerScanner : MonoBehaviour
{
    [Header("Trigger Elements")]
    public SphereCollider triggerArea = null;

    [Header("Trigger AI Elements")]
    public GameObject currentTarget = null;
    public bool isTargeting = false;
    public float triggerInterval = 0.1f;
    public float triggerRadius = 0.5f;

    [Header("ETC")]
    private bool monsterDelisted = false;
    private bool loopflag_TargetEnemy = false;
    public List<GameObject> monsterEnlisted;

    private IEnumerator TargetEnemy_Coroutine()
    {
        if(!loopflag_TargetEnemy)
        {
            loopflag_TargetEnemy = true;

            if(!monsterDelisted)
                TargetEnemy_Enlisted();

            yield return new WaitForSeconds(triggerInterval);

            loopflag_TargetEnemy = false;
        }
    }

    private void TargetEnemy_Enlisted()
    {
        float _dist = triggerRadius + 1;

        foreach(GameObject listed in monsterEnlisted)
        {   
            float _tmp = Vector3.Distance(transform.position, listed.transform.position);

            if (_dist > _tmp)
            {
                _dist = _tmp;
                currentTarget = listed;
            }
        }
    }

    public void DelistMonster(GameObject monster)
    {
        monsterDelisted = true;

        if (monsterEnlisted.Contains(monster))
            monsterEnlisted.Remove(monster);

        monsterDelisted = false;
    }

    private void Awake()
    {
        triggerArea = GetComponent<SphereCollider>();

        if(triggerArea == null)
        {
            throw new System.Exception("Tower has no SphereCollider");
        }

        triggerArea.radius = triggerRadius;
    }

    private void Update()
    {
        triggerRadius = GameManager.Instance.towerTriggerRange;
        triggerInterval = GameManager.Instance.towerTriggerInterval;

        if(!monsterDelisted)
            StartCoroutine(TargetEnemy_Coroutine());

        if (currentTarget != null)
            Debug.DrawLine(transform.position, currentTarget.transform.position, Color.red);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") && other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            other.GetComponent<TEST_NavMeshMonster>().listedTower.Add(GetComponent<TowerScanner>());
            monsterEnlisted.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            other.GetComponent<TEST_NavMeshMonster>().listedTower.Remove(GetComponent<TowerScanner>());
            monsterEnlisted.Remove(other.gameObject);
        }
    }
}
