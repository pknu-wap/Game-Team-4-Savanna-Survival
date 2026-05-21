using UnityEngine;

public class EnemyBleed : Enemy
{
    [Header("Config")]
    [SerializeField] private float maxHp          = 50f;
    [SerializeField] private float damage         = 10f;
    [SerializeField] private float detectionRange = 9999f; // 맵 전체 감지
    [SerializeField] private float attackRange    = 2f;
    [SerializeField] private float attackInterval = 2f;

    [Header("Attack Telegraph")]
    [SerializeField] private GameObject attackIndicatorPrefab;

    [Header("Bleed")]
    [Tooltip("공격 1회당 부여할 출혈 스택 수 (상태이상 시스템 구현 후 사용)")]
    [SerializeField] private int   bleedStacks    = 1;
    [Tooltip("스택당 초당 데미지 (상태이상 시스템 구현 후 사용)")]
    [SerializeField] private float bleedDotDamage = 3f;
    [Tooltip("출혈 지속 시간(초) (상태이상 시스템 구현 후 사용)")]
    [SerializeField] private float bleedDuration  = 4f;

    [Header("Animation")]
    [SerializeField] private string dieClipName = "Die";

    private float      attackTimer;
    private bool       telegraph;
    private GameObject indicator;

    private Animator anim;

    private static readonly int ParamSpeed  = Animator.StringToHash("Speed");
    private static readonly int ParamAttack = Animator.StringToHash("Attack");
    private static readonly int ParamDie    = Animator.StringToHash("Die");

    protected override void Awake()
    {
        base.Awake();
        statManager.InitAttacker(maxHp, damage);
        currentHp = statManager.getStat(StatType.HEALTH).rawValue;
        SetNewWanderTarget();

        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        UpdateAnimator();
        UpdateFacing();
    }

    protected override bool IsPlayerInDetection() => true;

    protected override void Move()
    {
        if (player == null || !player.gameObject.activeInHierarchy) { Wander(); return; }

        float dist = Vector2.Distance(transform.position, player.position);

        if      (dist <= attackRange)    Attack();
        else if (dist <= detectionRange) Chase();
        else                             Wander();
    }

    // ── 이동 ─────────────────────────────────────────────────

    private void Chase()
    {
        MoveSmooth((player.position - transform.position).normalized * moveSpeed);
        attackTimer = 0f;
        telegraph   = false;
        HideIndicator();
    }

    // ── 공격 ─────────────────────────────────────────────────

    private void Attack()
    {
        MoveSmooth(Vector2.zero);
        attackTimer += Time.deltaTime;

        if (!telegraph && attackTimer >= attackInterval - 1f)
        {
            ShowIndicator();
            telegraph = true;
        }

        if (attackTimer >= attackInterval)
        {
            anim?.SetTrigger(ParamAttack);
            DamagePlayer(statManager.getStat(StatType.DAMAGE).calibratedValue);
            ApplyBleed();
            attackTimer = 0f;
            telegraph   = false;
            HideIndicator();
        }
    }

    /// <summary>
    /// 플레이어에게 출혈 상태이상을 부여합니다.
    /// 상태이상 시스템 구현 후 이 메서드 내부만 채우면 됩니다.
    /// 예시:
    ///   StatusEffectManager.Instance.Apply(player, new BleedEffect(bleedStacks, bleedDotDamage, bleedDuration));
    /// </summary>
    private void ApplyBleed()
    {
        // TODO: 상태이상 시스템 연동
        // 현재는 출혈 적용 의도를 로그로만 남김
        Debug.Log($"[EnemyBleed] 출혈 부여 시도 — 스택:{bleedStacks}, DPS:{bleedDotDamage}, 지속:{bleedDuration}s");
    }

    // ── 사망 ─────────────────────────────────────────────────

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        ApplyDeathRewards();

        anim?.SetTrigger(ParamDie);
        if (indicator != null) Destroy(indicator);

        StartCoroutine(DieRoutine(GetDieClipLength(anim, dieClipName)));
    }

    // ── 애니메이터 / 방향 ─────────────────────────────────────

    private void UpdateAnimator()
    {
        if (anim == null) return;
        anim.SetFloat(ParamSpeed, rb.linearVelocity.magnitude);
    }

    private void UpdateFacing()
    {
        if (Mathf.Abs(velocity.x) < 0.05f) return;
        Vector3 s = transform.localScale;
        s.x = velocity.x > 0 ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
        transform.localScale = s;
    }

    // ── 인디케이터 ────────────────────────────────────────────

    private void ShowIndicator()
    {
        if (attackIndicatorPrefab == null) return;
        indicator ??= Instantiate(attackIndicatorPrefab, transform);
        indicator.transform.localPosition = Vector3.zero;
        indicator.transform.localScale    = Vector3.one * attackRange * 2f;
        indicator.SetActive(true);
    }

    private void HideIndicator()
    {
        if (indicator != null) indicator.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue; Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;  Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}