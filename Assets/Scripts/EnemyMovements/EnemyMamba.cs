using UnityEngine;

/// 블랙 맘바 컨셉 정예 몬스터. 공격 로직은 EnemyMambaAttacks.cs(partial)에 분리.
public partial class EnemyMamba : Enemy
{
    [Header("Config")]
    [SerializeField] private float maxHp          = 300f;
    [SerializeField] private float damage         = 20f;
    [SerializeField] private float detectionRange = 8f;

    [Header("Bite")]
    [SerializeField] private float biteRange    = 1.8f;
    [SerializeField] private float biteInterval = 1.5f;
    [SerializeField] private GameObject biteIndicatorPrefab;

    [Header("Spit")]
    [SerializeField] private float spitRange     = 7f;
    [SerializeField] private float spitInterval  = 5f;
    [SerializeField] private float spitTelegraph = 0.8f;
    [SerializeField] private GameObject spitProjectilePrefab;
    [SerializeField] private GameObject spitIndicatorPrefab;

    [Header("Poison Zone")]
    [SerializeField] private GameObject poisonZonePrefab;
    [SerializeField] private float poisonPercent      = 0.02f;
    [SerializeField] private float poisonTickInterval = 1f;
    [SerializeField] private float poisonDuration     = 5f;
    [SerializeField] private float spitZoneRadius     = 1.5f;
    [SerializeField] private float spitZoneDuration   = 6f;
    [SerializeField] private float deathZoneRadius    = 4f;
    [SerializeField] private float deathZoneDuration  = 8f;

    [Header("Animation")]
    [SerializeField] private string dieClipName = "mamba_die";

    private Animator anim;
    private static readonly int ParamSpeed = Animator.StringToHash("Speed");
    private static readonly int ParamBite  = Animator.StringToHash("Bite");
    private static readonly int ParamSpit  = Animator.StringToHash("Spit");
    private static readonly int ParamDie   = Animator.StringToHash("Die");

    internal enum State { Wander, Chase, Bite, SpitTelegraph, Spit }
    internal State state = State.Wander;

    internal float biteTimer;
    internal float spitTimer;
    internal float patternTimer;

    protected override void Awake()
    {
        base.Awake();
        statManager.InitAttacker(maxHp, damage);
        currentHp = statManager.getStat(StatType.HEALTH).rawValue;
        spitTimer = spitInterval * 0.5f;
        SetNewWanderTarget();
        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        anim?.SetFloat(ParamSpeed, rb.linearVelocity.magnitude);
        UpdateFacing();
    }

    protected override bool IsPlayerInDetection() => true;

    protected override void Move()
    {
        if (player == null || !player.gameObject.activeInHierarchy) { Wander(); return; }

        if (state != State.SpitTelegraph && state != State.Spit) spitTimer += Time.deltaTime;

        float dist = Vector2.Distance(transform.position, player.position);
        switch (state)
        {
            case State.Wander:        UpdateWander(dist);    break;
            case State.Chase:         UpdateChase(dist);     break;
            case State.Bite:          UpdateBite(dist);      break;
            case State.SpitTelegraph: UpdateSpitTelegraph(); break;
            case State.Spit:
                FireSpitProjectile();
                state = State.Chase;
                break;
        }
    }

    private void UpdateWander(float dist)
    {
        if (dist <= detectionRange) { state = State.Chase; return; }
        Wander();
    }

    private void UpdateChase(float dist)
    {
        if (spitTimer >= spitInterval && dist <= spitRange) { EnterSpitTelegraph(); return; }
        if (dist <= biteRange) { state = State.Bite; biteTimer = 0f; return; }
        if (dist > detectionRange) { state = State.Wander; SetNewWanderTarget(); return; }
        MoveSmooth((player.position - transform.position).normalized * moveSpeed);
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        ApplyDeathRewards();

        if (poisonZonePrefab != null)
            Instantiate(poisonZonePrefab, transform.position, Quaternion.identity)
                .GetComponent<PoisonZone>()?.Init(poisonPercent, poisonTickInterval, poisonDuration, deathZoneDuration, deathZoneRadius);

        anim?.SetTrigger(ParamDie);
        HideBiteIndicator();
        HideSpitIndicator();
        StartCoroutine(DieRoutine(GetDieClipLength(anim, dieClipName)));
    }

    internal void SpawnPoisonZone(Vector2 pos, float radius, float zoneDuration)
    {
        if (poisonZonePrefab == null) return;
        Instantiate(poisonZonePrefab, pos, Quaternion.identity)
            .GetComponent<PoisonZone>()?.Init(poisonPercent, poisonTickInterval, poisonDuration, zoneDuration, radius);
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
        Gizmos.color = Color.red;     Gizmos.DrawWireSphere(transform.position, biteRange);
        Gizmos.color = Color.green;   Gizmos.DrawWireSphere(transform.position, spitRange);
        Gizmos.color = Color.magenta; Gizmos.DrawWireSphere(transform.position, deathZoneRadius);
    }
}