using UnityEngine;

/// 사자 보스 본체.
/// 패턴 구현은 BossLionWild.cs / BossLionMagic.cs (partial) 에 분리.
public partial class BossLion : Enemy
{
    public enum SkillTree { Wild, Magic }

    [Header("Boss Tree")]
    [Tooltip("맵에서 스폰할 때 어떤 트리를 사용할지 지정")]
    [SerializeField] internal SkillTree activeTree = SkillTree.Wild;

    [Header("Config")]
    [SerializeField] private float maxHp  = 2000f;
    [SerializeField] private float damage = 30f;

    [Header("Melee")]
    [SerializeField] internal float      meleeRange          = 2.2f;
    [SerializeField] internal float      meleeInterval       = 1.8f;
    [SerializeField] internal float      meleeTelegraphRatio = 0.5f;
    [SerializeField] internal GameObject meleeIndicatorPrefab;

    // ── 상태머신 ─────────────────────────────────────────────
    internal enum State { Chase, Melee, Pattern }
    internal State state = State.Chase;

    internal float patternTimer;
    internal float patternCooldown;
    internal float meleeTimer;
    internal int   activePatternId = -1;

    // ── 레지스트리 참조 ──────────────────────────────────────
    internal BossSkillRegistry skillRegistry;

    // ── 애니메이터 ───────────────────────────────────────────
    private Animator anim;
    private static readonly int AnimSpeed  = Animator.StringToHash("Speed");
    private static readonly int AnimAttack = Animator.StringToHash("Attack");
    private static readonly int AnimDie    = Animator.StringToHash("Die");

    // ── 초기화 ───────────────────────────────────────────────

    protected override void Awake()
    {
        base.Awake();
        statManager.InitAttacker(maxHp, damage);
        currentHp = statManager.getStat(StatType.HEALTH).rawValue;

        skillRegistry = GetComponent<BossSkillRegistry>();
        skillRegistry?.Init(statManager);

        anim = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
        InitPatternCooldowns();
        Debug.Log($"[BossLion] 사용 트리: {activeTree}");
    }

    protected override bool IsPlayerInDetection() => true;

    // ── 메인 루프 ────────────────────────────────────────────

    protected override void Move()
    {
        if (player == null || !player.gameObject.activeInHierarchy) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // 마법 트리 긴급 인터럽트 — HP 임계값 도달 시 진행 중 패턴 취소 후 M2 강제 발동
        if (activeTree == SkillTree.Magic) CheckMagicInterrupt();

        switch (state)
        {
            case State.Chase:   UpdateChase(dist);  break;
            case State.Melee:   UpdateMelee(dist);  break;
            case State.Pattern: UpdatePattern();    break;
        }

        anim?.SetFloat(AnimSpeed, velocity.magnitude);

        if (velocity.sqrMagnitude > 0.01f)
            skillRegistry?.UpdateMoveDirection(velocity);
    }

    // ── 상태별 업데이트 ──────────────────────────────────────

    private void UpdateChase(float dist)
    {
        patternTimer += Time.deltaTime;

        if (patternTimer >= patternCooldown && CanStartPattern(dist))
        {
            EnterPattern();
            return;
        }

        if (dist <= meleeRange)
        {
            state = State.Melee;
            meleeTimer = 0f;
            return;
        }

        MoveSmooth((player.position - transform.position).normalized * moveSpeed);
    }

    private void UpdateMelee(float dist)
    {
        if (dist > meleeRange)
        {
            HideMeleeIndicator();
            state = State.Chase;
            return;
        }

        MoveSmooth(Vector2.zero);
        meleeTimer += Time.deltaTime;

        if (meleeTimer >= meleeInterval * (1f - meleeTelegraphRatio))
            ShowMeleeIndicator();

        if (meleeTimer >= meleeInterval)
        {
            HideMeleeIndicator();
            float dmg = statManager.getStat(StatType.DAMAGE).calibratedValue;
            DamagePlayer(dmg);
            anim?.SetTrigger(AnimAttack);
            meleeTimer = 0f;
            Debug.Log($"[BossLion] 근접 공격 — 데미지 {dmg:F1}");
        }
    }

    // ── 패턴 진입/종료 ───────────────────────────────────────

    private void EnterPattern()
    {
        state        = State.Pattern;
        patternTimer = 0f;
        MoveSmooth(Vector2.zero);
        StartNextPattern();
    }

    internal void ExitPattern()
    {
        activePatternId = -1;
        patternTimer    = 0f; // ★ 패턴 종료 시 쿨다운 리셋 — 즉시 재진입 방지
        state           = State.Chase;
    }

    // ── 스킬 레지스트리 패턴 실행 ────────────────────────────

    /// <summary>
    /// 레지스트리의 Active 슬롯을 순서대로 순환하며 다음 준비된 스킬을 실행합니다.
    /// 실행 시 Attack 트리거도 발동합니다.
    /// </summary>
    internal bool TryExecuteNextSkill()
    {
        if (skillRegistry == null) return false;
        bool executed = skillRegistry.TryExecuteNextActive();
        if (executed)
            anim?.SetTrigger(AnimAttack);
        return executed;
    }

    /// <summary>
    /// 레지스트리에 준비된 Active 슬롯이 하나라도 있는지 확인합니다.
    /// 패턴 진입 조건 체크에 사용합니다.
    /// </summary>
    internal bool HasSkillReady()
        => skillRegistry != null && skillRegistry.HasAnyActiveReady();


    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        anim?.SetTrigger(AnimDie);
        ApplyDeathRewards();
        HideMeleeIndicator();
        OnBossDeath();
        StartCoroutine(DieRoutine(0f));
    }

    // ── 업데이트 / 방향 ──────────────────────────────────────

    protected override void Update()
    {
        base.Update();
        UpdateFacing();
    }

    private void UpdateFacing()
    {
        if (Mathf.Abs(velocity.x) < 0.05f) return;
        Vector3 s = transform.localScale;
        s.x = velocity.x > 0 ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
        transform.localScale = s;
    }

    // ── 근접 인디케이터 ──────────────────────────────────────

    private GameObject meleeIndicator;

    private void ShowMeleeIndicator()
    {
        if (meleeIndicatorPrefab == null) return;
        if (meleeIndicator == null)
        {
            meleeIndicator = Instantiate(meleeIndicatorPrefab, transform);
            meleeIndicator.transform.localPosition = Vector3.zero;
            meleeIndicator.transform.localScale    = Vector3.one * meleeRange * 2f;
        }
        meleeIndicator.SetActive(true);
    }

    internal void HideMeleeIndicator()
    {
        if (meleeIndicator != null) meleeIndicator.SetActive(false);
    }

    // ── 기즈모 ───────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }

    // ── 패턴 분기 ────────────────────────────────────────────

    private void InitPatternCooldowns()
    {
        if (activeTree == SkillTree.Wild) InitWildCooldowns();
        else                              InitMagicCooldowns();
    }

    private void StartNextPattern()
    {
        if (activeTree == SkillTree.Wild) StartNextWildPattern();
        else                              StartNextMagicPattern();
    }

    private void UpdatePattern()
    {
        if (activeTree == SkillTree.Wild) UpdateWildPattern();
        else                              UpdateMagicPattern();
    }

    private bool CanStartPattern(float dist)
    {
        return activeTree == SkillTree.Wild
            ? CanStartWildPattern(dist)
            : CanStartMagicPattern(dist);
    }

    private void OnBossDeath()
    {
        if (activeTree == SkillTree.Wild) OnWildBossDeath();
        else                              OnMagicBossDeath();
    }
}