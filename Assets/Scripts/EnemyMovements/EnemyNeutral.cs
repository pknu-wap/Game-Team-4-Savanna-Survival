using UnityEngine;

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
    [Tooltip("물 분사 시작 위치를 몸 중앙에서 앞쪽으로 얼마나 밀지 설정 (월드 단위)")]
    [SerializeField] private float waterSpawnOffset = 1f;
    [SerializeField] private GameObject waterZonePrefab;

    [Header("Animation")]
    [SerializeField] private string dieClipName = "Die";

    private enum AttackState { Ready, AoeWaiting, AoeTelegraph, Aoe, Water, Melee }
    private AttackState state = AttackState.Ready;

    private bool  isHostile;
    private float meleeTimer;
    private float aoeTimer;
    private float waterTimer;
    private float patternTimer;

    // EnemyNeutralAttacks.cs에서도 참조
    protected Animator anim;

    private static readonly int ParamSpeed        = Animator.StringToHash("Speed");
    private static readonly int ParamIsMelee      = Animator.StringToHash("IsMelee");
    private static readonly int ParamIsAoeTelegraph = Animator.StringToHash("IsAoeTelegraph");
    private static readonly int ParamIsWater      = Animator.StringToHash("IsWater");
    private static readonly int ParamDie          = Animator.StringToHash("Die");

    protected override void Awake()
    {
        base.Awake();
        statManager.InitAttacker(maxHp, damage);
        currentHp = statManager.getStat(StatType.HEALTH).rawValue;
        aoeTimer  = aoeInterval * 0.5f;
        SetNewWanderTarget();
        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        anim?.SetFloat(ParamSpeed, rb.linearVelocity.magnitude);
        UpdateFacing();
    }

    public override void TakeDamage(float dmg)
    {
        isHostile = true;
        base.TakeDamage(dmg);
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        ApplyDeathRewards();
        anim?.SetTrigger(ParamDie);
        DestroyWaterIndicator();
        StartCoroutine(DieRoutine(GetDieClipLength(anim, dieClipName)));
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
        SetAttackBools(false, false, false);

        if      (aoeTimer >= aoeInterval)                           EnterAoeWaiting();
        else if (dist >= waterRange && waterTimer >= waterCooldown) EnterWater();
        else if (dist <= meleeRange)                                state = AttackState.Melee;
        else if (dist <= detectionRange)                            MoveSmooth((player.position - transform.position).normalized * moveSpeed);
        else                                                        Wander();
    }

    // 공격 상태 Bool을 한 번에 세팅 — Ready 전환 시 일괄 초기화에 사용
    private void SetAttackBools(bool melee, bool aoe, bool water)
    {
        anim?.SetBool(ParamIsMelee,         melee);
        anim?.SetBool(ParamIsAoeTelegraph,  aoe);
        anim?.SetBool(ParamIsWater,         water);
    }

    private void UpdateFacing()
    {
        if (Mathf.Abs(velocity.x) < 0.05f) return;
        Vector3 s = transform.localScale;
        s.x = velocity.x > 0 ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
        transform.localScale = s;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;    Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;     Gizmos.DrawWireSphere(transform.position, meleeRange);
        Gizmos.color = Color.magenta; Gizmos.DrawWireSphere(transform.position, aoeRange);
        Gizmos.color = Color.cyan;    Gizmos.DrawWireSphere(transform.position, waterRange);
    }
}