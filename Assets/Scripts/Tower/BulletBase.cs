using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    [Header("References")]
    public Collider bulletCollider = null;
    public TowerBase motherObject = null;
    public GameObject targetMonster = null;
    public GameObject explodeEffect = null;

    [Header("Variables")]
    public int damage = 0;
    public Vector3 targetCoord = Vector3.zero;
    public float bulletSpeed = 0f;
    public bool isTriggered = false;

    private void InitializeBullet()
    {
        if (bulletCollider == null)
            throw new System.Exception(nameof(BulletBase) + " has no collider");

        if (motherObject == null)
            throw new System.Exception(nameof(BulletBase) + " has no mother object");

        if (explodeEffect == null)
            throw new System.Exception(nameof(BulletBase) + " has no explode effect");
    }

    private void Start()
    {
        InitializeBullet();
    }

    private void InstantiateEffect()
    {
        GameObject tmpEff = Instantiate(explodeEffect, transform.position, Quaternion.identity);

        if (motherObject.type == "BIGCANNON")
        {
            float _radius = motherObject.scanner.triggerRadius * 2;
            tmpEff.transform.localScale = new Vector3(_radius, _radius, _radius);
        }
            

        if (motherObject.type == "MACHINEGUN")
            tmpEff.transform.localScale = new Vector3(2, 2, 2);

        Destroy(tmpEff, 3f);
    }

    private void AreaDamage(Collider center)
    {
        Collider[] targetInRange = Physics.OverlapSphere(center.transform.position,motherObject.scanner.triggerRadius * 1.5f,LayerMask.GetMask("Enemy"));

        foreach(Collider target in targetInRange)
        {
            target.GetComponent<TEST_NavMeshMonster>().MonsterHitByEmeny(damage / 4, motherObject.gameObject);
        }
    }

    private void Update()
    {
        if(!isTriggered)
        {
            if(targetMonster != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetMonster.transform.position, bulletSpeed);
            }
            else
            {
                gameObject.SetActive(false);
                motherObject.isCooldown = false;
            } 
        }
    } 

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy") && other.CompareTag("Enemy"))
        {
            InstantiateEffect();
            isTriggered = true;

            if(motherObject.type == "MACHINEGUN")
                other.GetComponent<TEST_NavMeshMonster>().MonsterHitByEmeny(damage, motherObject.gameObject);

            if (motherObject.type == "BIGCANNON")
                AreaDamage(other);

            gameObject.SetActive(false);
            motherObject.isCooldown = false;
        }
    }
}
