using UnityEngine;

public partial class EnemyBison : Enemy
{
    [Header("Config")]
    [SerializeField] private float maxHp          = 120f;
    [SerializeField] private float damage         = 15f;
    [SerializeField] private float detectionRange = 7f;

    [Header("Melee")]
    [SerializeField] private float meleeRange    = 1.8f;
    [SerializeField] private float meleeInterval = 1.8f;
    [SerializeField] private GameObject meleeIndicatorPrefab;

    [Header("Charge")]
    [Tooltip("돌진 감지 범위 — detectionRange 보다 크게 설정")]
    [SerializeField] private float chargeDetectionRange = 13f;
    [SerializeField] private float chargeCooldown       = 10f;
    [SerializeField] private float chargeTelegraph      = 1.2f;
    [SerializeField] private float chargeSpeed          = 18f;
    [SerializeField] private float chargeMaxDuration    = 1.5f;
    [SerializeField] private float chargeDamage         = 30f;
    [SerializeField] private float chargeRecoveryTime   = 0.8f;
    [SerializeField] private GameObject chargeIndicatorPrefab;

    [Header("Animation")]
    [SerializeField] private string dieClipName = "Die";

    protected Animator anim;

    private static readonly int ParamSpeed  = Animator.StringToHash("Speed");
    private static readonly int ParamAttack = Animator.StringToHash("Attack");
    private static readonly int ParamCharge = Animator.StringToHash("Charge");
    private static readonly int ParamDie    = Animator.StringToHash("Die");

    private enum BisonState { Wander, Chase, Melee, ChargeTelegraph, Charging, ChargeRecovery }
    private BisonState state = BisonState.Wander;

    private float   meleeTimer;
    private float   chargeTimer;
    private float   patternTimer;
    private Vector2 chargeDir;
    private bool    chargeDamageDealt;

    protected override void Awake()
    {
        base.Awake();
        statManager.InitAttacker(maxHp, damage);
        currentHp   = statManager.getStat(StatType.HEALTH).rawValue;
        chargeTimer = chargeCooldown * 0.5f;
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

        bool inChargePattern = state == BisonState.ChargeTelegraph
                            || state == BisonState.Charging
                            || state == BisonState.ChargeRecovery;
        if (!inChargePattern) chargeTimer += Time.deltaTime;

        float dist = Vector2.Distance(transform.position, player.position);

        switch (state)
        {
            case BisonState.Wander:          UpdateWander(dist);      break;
            case BisonState.Chase:           UpdateChase(dist);       break;
            case BisonState.Melee:           UpdateMelee(dist);       break;
            case BisonState.ChargeTelegraph: UpdateChargeTelegraph(); break;
            case BisonState.Charging:        UpdateCharging();        break;
            case BisonState.ChargeRecovery:  UpdateChargeRecovery(); break;
        }
    }

    private void UpdateWander(float dist)
    {
        if (chargeTimer >= chargeCooldown && dist <= chargeDetectionRange) { EnterChargeTelegraph(); return; }
        if (dist <= detectionRange) { state = BisonState.Chase; return; }
        Wander();
    }

    private void UpdateChase(float dist)
    {
        if (chargeTimer >= chargeCooldown && dist <= chargeDetectionRange) { EnterChargeTelegraph(); return; }
        if (dist <= meleeRange) { state = BisonState.Melee; meleeTimer = 0f; return; }
        if (dist > detectionRange) { state = BisonState.Wander; SetNewWanderTarget(); return; }
        MoveSmooth((player.position - transform.position).normalized * moveSpeed);
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        ApplyDeathRewards();
        anim?.SetTrigger(ParamDie);
        HideMeleeIndicator();
        HideChargeIndicator();
        StartCoroutine(DieRoutine(GetDieClipLength(anim, dieClipName)));
    }

    private void UpdateFacing()
    {
        float xDir = state == BisonState.Charging ? chargeDir.x : velocity.x;
        if (Mathf.Abs(xDir) < 0.05f) return;
        Vector3 s = transform.localScale;
        s.x = xDir > 0 ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
        transform.localScale = s;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;   Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;    Gizmos.DrawWireSphere(transform.position, meleeRange);
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, chargeDetectionRange);
    }
}