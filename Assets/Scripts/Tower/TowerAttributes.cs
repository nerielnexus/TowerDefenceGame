using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="TowerAttributes",menuName ="Scriptable Object/Tower Attributes",order =1)]
public class TowerAttributes : ScriptableObject
{
    // attributes - basic
    public string type = null;                              // 타워의 종류, BIGCANNON SMALLCANNON MUSHROOM ...
    public string attackVariantType = null;                 // 공격의 종류, SINGLE(1인 단일), AREA(범위 공격) ...
    public float attackInterval = 0f;                       // 공격 쿨타임
    public int attackRepeat = 0;                            // 공격 회수
    public int normalAttackDamage = 0;                      // 공격 대미지
    public float rotateSpeed = 0f;                          // 포탑이 회전하는 경우 그 회전 속도
    public float bulletSpeed = 0f;                          // 탄속
    public int buildCost = 0;                               // 건설 비용

    // attributes - increased by level?
}
