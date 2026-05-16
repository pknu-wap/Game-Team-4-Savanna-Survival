using UnityEngine;

/// 공격 패턴 로직을 EnemyNeutralAttacks.cs 에 있는 partial 클래스로 분리함.
public partial class EnemyNeutral : Enemy
{
    [Header("Config")]
    [SerializeField] private float maxHp          = 200f;
    [SerializeField] private float damage         = 25f;
    [SerializeField] private float detectionRange = 8f;

    [Header("Melee")]
    [SerializeField] private float meleeRange    = 2f;
    [SerializeField] private float meleeInterval = 1.5f;
    [SerializeField] private GameObject indicatorPrefab;

    [Header("AoE")]
    [SerializeField] private float aoeRange     = 4f;
    [SerializeField] private float aoeInterval  = 8f;
    [SerializeField] private float aoeTelegraph = 2.5f;
    [SerializeField] private float aoeDamage    = 40f;

    [Header("Water")]
    [SerializeField] private float waterRange     = 7f;
    [SerializeField] private float waterDuration  = 3f;
    [SerializeField] private float waterCooldown  = 5f;
    [SerializeField] private float waterTelegraph = 0.8f;
    [SerializeField] private float waterDamage    = 8f;
    [SerializeField] private float waterWidth     = 1.5f;
    [SerializeField] private GameObject waterZonePrefab;

    private enum AttackState { Ready, AoeWaiting, AoeTelegraph, Aoe, Water, Melee }
    private AttackState state = AttackState.Ready;

    private bool  isHostile;
    private float meleeTimer;
    private float aoeTimer;
    private float waterTimer;
    private float patternTimer;

    protected override void Awake()
    {
        base.Awake();
        statManager.InitAttacker(maxHp, damage);
        currentHp = statManager.getStat(StatType.HEALTH).rawValue;
        aoeTimer  = aoeInterval * 0.5f;
        SetNewWanderTarget();
    }

    public override void TakeDamage(float dmg)
    {
        isHostile = true;
        base.TakeDamage(dmg);
    }

    protected override void Die()
    {
        DestroyWaterIndicator();
        base.Die();
    }

    protected override bool IsPlayerInDetection() => isHostile;

    protected override void Move()
    {
        if (player == null || !player.gameObject.activeInHierarchy || !isHostile) { Wander(); return; }

        aoeTimer   += Time.deltaTime;
        waterTimer += Time.deltaTime;

        float dist = Vector2.Distance(transform.position, player.position);

        switch (state)
        {
            case AttackState.Ready:        UpdateReady(dist);      break;
            case AttackState.AoeWaiting:   UpdateAoeWaiting(dist); break;
            case AttackState.AoeTelegraph: UpdateAoeTelegraph();   break;
            case AttackState.Aoe:          UpdateAoe(dist);        break;
            case AttackState.Water:        UpdateWater();          break;
            case AttackState.Melee:        UpdateMelee(dist);      break;
        }
    }

    private void UpdateReady(float dist)
    {
        if      (aoeTimer >= aoeInterval)                           EnterAoeWaiting();
        else if (dist >= waterRange && waterTimer >= waterCooldown) EnterWater();
        else if (dist <= meleeRange)                                state = AttackState.Melee;
        else if (dist <= detectionRange)                            MoveSmooth((player.position - transform.position).normalized * moveSpeed);
        else                                                        Wander();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;    Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;     Gizmos.DrawWireSphere(transform.position, meleeRange);
        Gizmos.color = Color.magenta; Gizmos.DrawWireSphere(transform.position, aoeRange);
        Gizmos.color = Color.cyan;    Gizmos.DrawWireSphere(transform.position, waterRange);
    }
}