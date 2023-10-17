using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="MonsterAttributes",menuName ="Scriptable Object/Monster Attributes",order =2)]
public class MonsterAttributes : ScriptableObject
{
    // Attributes - Health
    public float healthMaximum = 0;
    public float healthRegenRate = 0;
    public float healthLowLifeRatio = 0;

    // Attributes - Movement
    public float speedWalk = 0;
    public float speedRotate = 0;
    public float speedModifierRun = 0;
    public float speedModifierCrawl = 0;

    // Attributes - Offense
    public float attackDamageMelee = 0;
    public float attackDamageRange = 0;
    public float bigAttackDamageModifier = 0;
    public float chanceToBigAttack = 0;

    // Attributes - Defense
    public float armourMelee = 0;
    public float armourRange = 0;
    public float armourShield = 0;
    public float damageReductionRatio = 0;

    // Attributes - Rewards
    public int baseCreditRewards = 0;
    public float creditBoostRatioMin = 0f;
    public float creditBoostRatioMax = 0f;
}
