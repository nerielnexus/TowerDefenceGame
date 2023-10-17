using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TowerBase : MonoBehaviour
{
    [Header("Tower Elements")]
    public TowerScanner scanner = null;
    public GameObject bullet = null;
    public GameObject currentTarget = null;
    public GameObject barrel = null;
    public Transform barrelPoint = null;
    public List<GameObject> magazine = new List<GameObject>();

    [Header("Tower Attributes - Original")]
    public TowerAttributes attributes = null;

    [Header("Tower Attributes - Copied From Original")]
    public string type = null;
    public string attackVariantType = null;
    public float attackInterval = 0f;
    public int attackRepeat = 0;
    public int normalAttackDamage = 0;
    public float rotateSpeed = 0f;
    public float bulletSpeed = 0f;
    public int buildCost = 0;

    [Header("Flags , et cetera")]
    public bool isBarrelRotate = false;
    protected bool loopflag = false;
    public bool isCooldown = false;

    protected void InitializeTowerData()
    {
        if (scanner == null)
            throw new System.Exception(nameof(TowerBase) + " - " + nameof(TowerScanner) + " is null");

        if (bullet == null)
            throw new System.Exception(nameof(TowerBase) + " - Bullet is null");

        if (attributes == null)
            throw new System.Exception(nameof(TowerBase) + " - " + nameof(TowerAttributes) + " is null");

        type = attributes.type;
        attackVariantType = attributes.attackVariantType;
        attackInterval = attributes.attackInterval
            * (float)System.Math.Pow(UpgradeManager.Instance.towerUpgradeModifier_AttackInterval,UpgradeManager.Instance.towerUpgradeLevel_AttackInterval);
        normalAttackDamage = attributes.normalAttackDamage;
        rotateSpeed = attributes.rotateSpeed;
        attackRepeat = attributes.attackRepeat;
        bulletSpeed = attributes.bulletSpeed;
        buildCost = attributes.buildCost;

        if (barrel == null)
            throw new System.Exception(nameof(TowerBase) + " - Barrel is null");

        if (barrelPoint == null)
            throw new System.Exception(nameof(TowerBase) + " - Barrel Point is null");

        isBarrelRotate = (type == "BIGCANNON" || type == "MACHINEGUN") ? true : false;
    }

    private void RotateBarrel()
    {
        if (!isBarrelRotate || currentTarget == null)
            return;

        Vector3 barrelDirection = currentTarget.transform.position - barrel.transform.position;
        Quaternion barrelRotation = Quaternion.LookRotation(barrelDirection);
        barrel.transform.rotation = Quaternion.Slerp(barrel.transform.rotation, barrelRotation, rotateSpeed * Time.deltaTime);
        barrel.transform.rotation = Quaternion.Euler(new Vector3(0f, barrel.transform.rotation.eulerAngles.y, 0));
    }

    private IEnumerator NormalAttackLoop_SingleShots()
    {
        if (!loopflag)
        {
            loopflag = true;

            if (!isCooldown && currentTarget != null)
            {
                if (magazine.Count <= 0)
                {
                    for (int i = 0; i < attackRepeat; i++)
                    {
                        GameObject tmpBullet = Instantiate(bullet, barrelPoint.position, Quaternion.identity);
                        tmpBullet.GetComponent<BulletBase>().motherObject = gameObject.GetComponent<TowerBase>();
                        tmpBullet.name = bullet.name + $"{i + 1}" + " " + type;

                        if (type == "BIGCANNON")
                            tmpBullet.transform.localScale = new Vector3(4, 4, 4);

                        magazine.Add(tmpBullet);
                    }
                }

                // 탄환의 초기 값 설정
                foreach (GameObject bullet in magazine)
                {
                    bullet.transform.position = barrelPoint.position;
                    bullet.transform.rotation = Quaternion.identity;

                    bullet.GetComponent<BulletBase>().damage = normalAttackDamage;
                    bullet.GetComponent<BulletBase>().targetMonster = currentTarget;
                    bullet.GetComponent<BulletBase>().bulletSpeed = bulletSpeed;

                    // 탄환의 활성화 상태(activeSelf)와 적에게 닿았는지(isTrigger) 를 확인하고 값 수정하기
                    if (!bullet.activeSelf)
                        bullet.SetActive(true);

                    if (bullet.GetComponent<BulletBase>().isTriggered)
                        bullet.GetComponent<BulletBase>().isTriggered = false;

                    if(type == "MACHINEGUN")
                        yield return new WaitForSeconds(attackInterval * 0.2f);
                }

                isCooldown = true;
            }
            else
            {
                //if (magazine.All(x => x.GetComponent<BulletBase>().isTriggered == true))
                if(magazine.All(x => !x.activeSelf))
                {
                    foreach (GameObject bullet in magazine)
                    {
                        bullet.GetComponent<BulletBase>().isTriggered = false;
                    }

                    isCooldown = false;
                }
            }

            float _interval = (type == "MACHINEGUN") ? attackInterval * 1.5f : attackInterval;
            yield return new WaitForSeconds(_interval);

            loopflag = false;
        }
    }

    private void NormalAttack()
    {
        if(type == "BIGCANNON" || type == "MACHINEGUN")
        {
            StartCoroutine(NormalAttackLoop_SingleShots());
        }
    }

    private void Awake()
    {
        InitializeTowerData();
    }

    private void Update()
    {
        currentTarget = scanner.currentTarget;

        RotateBarrel();
        NormalAttack();
    }
}
