using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class TEST_NavMeshMonster : MonoBehaviour
{
    [Header("Monster AI Elements")]
    public NavMeshAgent mobAgent = null;
    public int mobAreaMask = 1;
    public MapManager mmgr = null;
    public float playerAwareDistance = 5.0f;
    public GameObject player = null;
    public Vector3 destCoord = Vector3.zero;
    public bool isTargetPlayer = false;
    public bool endOfRoad = false;

    [Header("Monster AI Elements - etc.")]
    private Rigidbody rb = null;
    public float wakeInterval = 0.1f;
    public float magnitudeThreshold = 0.3f;
    public float distanceMinimumThreshold = 0.3f;

    [Header("ETC")]
    private bool loopflag_SearchMoveLogics = false;
    public List<TowerScanner> listedTower = new List<TowerScanner>();

    [Header("test")]
    public bool isChasing;
    public bool isPathPending;
    public bool isOutOfRange;
    public bool hasSpeed;
    public float magnitude;
    public float remainDist;
    public NavMeshPathStatus status;

    [Header("Monster Attributes")]
    public MonsterAttributes mobAtt = null;

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

    [Header("Other Managers")]
    ScoreManager smgr = null;

    private void TEST()
    {
        isChasing = isTargetPlayer;
        isPathPending = mobAgent.pathPending;
        isOutOfRange = (mobAgent.remainingDistance > distanceMinimumThreshold);
        hasSpeed = (mobAgent.velocity.sqrMagnitude >= magnitudeThreshold);
        status = mobAgent.pathStatus;
        magnitude = mobAgent.velocity.sqrMagnitude;
        remainDist = mobAgent.remainingDistance;
    }

    /// <summary>
    /// 여러 조건들을 확인해 몬스터 객채가 end cube 에 도착했는지 확인한다
    /// </summary>
    /// <returns>몬스터 객체가 end cube 에 도착하면 true 를 반환</returns>
    public bool HasReachedDestinationInnerLogics()
    {
        // 몬스터 객체의 NavMeshAgent 를 확인해
        // destination 이 player 의 position 이라면 false 반환
        if (isTargetPlayer)
        {
            return false;
        }

        // navmeshagent 의 내부 기능인 '남은 거리(remainingDistance)' 와 '제동 거리(stoppingDistance)' 를 비교한다
        // 남은 거리가 더 크면 false 반환
        // vector3.distance(end,start) > 5.0f 같은 코드와 비슷한 역할을 한다
        if (mobAgent.remainingDistance > distanceMinimumThreshold)
        {
            return false;
        }

        /*
         * magnitude : 한 벡터의 크기 or 길이, 벡터의 각 성분을 소재로 계산한 피타고라스 수의 값
         * sqr magnitude : magnitude 를 제곱(square)한 값
         * 
         * 벡터와 운동의 표현
         * 벡터는 크기와 방향을 갖고 있으며, 물체는 벡터의 크기만큼의 속도와 벡터의 뱡향과 같은 이동 방향을 가짐
         * 즉 물체가 갖고 있는 벡터의 크기가 0 이라면 물체가 정지한 상태라고 할 수 있다
         * 
         * mganitudeThreshold
         * 근데 magnitude != 0f 하기엔 판정이 너무 야박하다
         * 그래서 사용자가 변경할 수 있는 판정 여유값, magnitudeThreshold 를 정의하고 벡터의 크기가 여유값보다 작으면
         * 판정상 멈췄다 처리하는 것
         */
        if(mobAgent.velocity.sqrMagnitude > magnitudeThreshold)
        {
            return false;
        }

        // 일정 거리 안에 들어왔고 속도가 일정 이하라면 사실상 도착했다고 취급하기
        mobAgent.ResetPath();

        return true;
    }

    /// <summary>
    /// 몬스터 객체와 플레이어 객체 사이의 거리를 계산하고, 몬스터 객체가 NavMesh 를 이용해 플레이어 객체에게 도달할 수 있는지 확인한다
    /// </summary>
    /// <returns>몬스터 객체가 플레이어 객체의 위치에 도달할 수 있으면 true 를 반환</returns>
    private bool CheckDistaiceBetweenPlayer()
    {
        // tag 가 Player 인 객체를 찾고, 없다면 null 로 한다
        player = GameObject.FindGameObjectWithTag("Player") ?? null;

        if(player == null)
        {
            return false;
        }

        // 몬스터 객체와 플레이어 객체 사이의 거리가 지정한 값(playerAwaerDistance) 보다 크면
        // 두 객체가 충분히 멀리 있어 '어그로가 끌리지 않은 상태' 로 판단하고 false 를 반환한다
        if(Vector3.Distance(transform.position,player.transform.position) > playerAwareDistance)
        {
            return false;
        }

        // 몬스터 객체가 NavMesh 를 이용해 플레이어 객체의 위치에 도달할 수 있는지 판단하고
        // 불가능하면 false 를 반환한다
        if(!mobAgent.CalculatePath(player.transform.position, new NavMeshPath()))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 몬스터 객체가 플레이어 객체에 접근할 수 있는지, end cube 에 도달했는지 확인할 메서드를 호출하는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator SearchMoveLogics()
    {
        // loopflag 변수를 이용해 매 프레임마다 코루틴이 호출되는 것을 방지
        if(!loopflag_SearchMoveLogics)
        {
            loopflag_SearchMoveLogics = true;

            MonsterMoves();

            yield return new WaitForSeconds(wakeInterval);

            loopflag_SearchMoveLogics = false;
        }
    }

    private void DelistFromOthers()
    {
        foreach(TowerScanner tower in listedTower)
        {
            tower.DelistMonster(gameObject);
        }

        GameObject.FindGameObjectWithTag("MobManager").GetComponent<MobManager>().DelistCurrentSpawnedMobs(GetComponent<TEST_NavMeshMonster>());

        listedTower.Clear();
        listedTower.TrimExcess();
    }

    private void SendKillInfo()
    {
        int resultCredit = Mathf.RoundToInt(baseCreditRewards * Random.Range(creditBoostRatioMin, creditBoostRatioMax));
 
        smgr.killCount++;
        smgr.GainCredit(resultCredit);
    }

    public void KillThisMonster(bool reached = false)
    {
        if(!reached)
            SendKillInfo();

        DelistFromOthers();
        Destroy(gameObject);
    }

    private void MonsterMoves()
    {
        if (CheckDistaiceBetweenPlayer())
        {
            // 설정한 값보다 가깝다면 NavMeshAgent 의 destination 을 플레이어 객체의 위치로 변경
            if (!mobAgent.destination.Equals(player.transform.position))
            {
                mobAgent.SetDestination(player.transform.position);
            }

            isTargetPlayer = true;
        }
        else
        {
            // 아니라면 길의 끝 end cube 로 설정
            if (!mobAgent.destination.Equals(destCoord))
            {
                mobAgent.SetDestination(destCoord);
            }

            isTargetPlayer = false;

            // end cube 에 도착했는지 확인
            if (HasReachedDestinationInnerLogics())
            {
                // 도착했다면 그에 맞는 처리를 실행
                KillThisMonster(true);
            }
        }
    }

    public void MonsterHitByEmeny(int damage, GameObject attacker)
    {
        Debug.Log(gameObject.name + " hit by " + attacker.name + ", dealt " + damage + " damage");

        if(healthMaximum > 0)
        {
            healthMaximum -= damage;
        }
        else
        {
            Debug.Log(gameObject.name + " killed by " + attacker.name);
            KillThisMonster();
        }
    }

    private void GetDataFromMonsterAttributes()
    {
        if (mobAtt == null)
            throw new System.Exception("Monster has no Attribute file (ScriptableObject");

        healthMaximum = mobAtt.healthMaximum;
        healthRegenRate = mobAtt.healthRegenRate;
        healthLowLifeRatio = mobAtt.healthLowLifeRatio;

        speedWalk = mobAtt.speedWalk;
        speedRotate = mobAtt.speedRotate;
        speedModifierRun = mobAtt.speedModifierRun;
        speedModifierCrawl = mobAtt.speedModifierCrawl;

        attackDamageMelee = mobAtt.attackDamageMelee;
        attackDamageRange = mobAtt.attackDamageRange;
        bigAttackDamageModifier = mobAtt.bigAttackDamageModifier;
        chanceToBigAttack = mobAtt.chanceToBigAttack;

        armourMelee = mobAtt.armourMelee;
        armourRange = mobAtt.armourRange;
        armourShield = mobAtt.armourShield;
        damageReductionRatio = mobAtt.damageReductionRatio;

        baseCreditRewards = mobAtt.baseCreditRewards;
        creditBoostRatioMin = mobAtt.creditBoostRatioMin;
        creditBoostRatioMax = mobAtt.creditBoostRatioMax;
    }

    private void Awake()
    {
        // 필요한 변수의 초기값을 설정함
        // 각종 컴포넌트가 있는지 확인하고 end cube 의 좌표를 참조해 도착 지점의 좌표 값을 설정하는 등
        mobAgent = GetComponent<NavMeshAgent>() ?? throw new System.Exception(nameof(TEST_NavMeshMonster) + "|" + gameObject.name + " has  no " + nameof(NavMeshAgent));
        mobAgent.acceleration = Random.Range(5f, 15f);
        mobAgent.speed = mobAgent.acceleration;

        mmgr = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>() ?? throw new System.Exception(nameof(TEST_NavMeshMonster) + "|" + gameObject.name + " has no " + nameof(MapManager));

        rb = GetComponent<Rigidbody>() ?? throw new System.Exception(nameof(TEST_NavMeshMonster) + "|" + gameObject.name + " has no " + nameof(Rigidbody));

        destCoord.x = mmgr.roadInFlow[mmgr.roadInFlow.Count - 1].GetComponent<Renderer>().bounds.center.x;
        destCoord.y = mmgr.roadInFlow[mmgr.roadInFlow.Count - 1].GetComponent<Renderer>().bounds.max.y;
        destCoord.z = mmgr.roadInFlow[mmgr.roadInFlow.Count - 1].GetComponent<Renderer>().bounds.center.z;

        smgr = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>() ?? throw new System.Exception(nameof(TEST_NavMeshMonster) + " cannot find " + nameof(ScoreManager));

        GetDataFromMonsterAttributes();

        // 초기 destination 을 end cube 의 좌표를 참조한 도착 지점 값으로 설정함
        mobAgent.SetDestination(destCoord);
    }

    private void Update()
    {
        MonsterMoves();
    }
}
