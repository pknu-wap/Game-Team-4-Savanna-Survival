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
    [SerializeField] private float maxHp          = 2000f;
    [SerializeField] private float damage         = 30f;
    [SerializeField] private float detectionRange = 15f;

    [Header("Melee")]
    [SerializeField] internal float meleeRange    = 2.2f;
    [SerializeField] internal float meleeInterval = 1.8f;
    [SerializeField] internal GameObject meleeIndicatorPrefab;

    internal enum State { Wander, Chase, Melee, Pattern }
    internal State state = State.Wander;

    internal float patternTimer;
    internal float patternCooldown;
    internal float meleeTimer;
    internal int   activePatternId = -1;

    protected override void Awake()
    {
        base.Awake();
        statManager.InitAttacker(maxHp, damage);
        currentHp = statManager.getStat(StatType.HEALTH).rawValue;
        SetNewWanderTarget();
    }

    protected override void Start()
    {
        base.Start();
        InitPatternCooldowns();
        Debug.Log($"[BossLion] 사용 트리: {activeTree}");
    }

    protected override bool IsPlayerInDetection() => true;

    protected override void Move()
    {
        if (player == null || !player.gameObject.activeInHierarchy) { Wander(); return; }

        float dist = Vector2.Distance(transform.position, player.position);
        switch (state)
        {
            case State.Wander:  UpdateWander(dist); break;
            case State.Chase:   UpdateChase(dist);  break;
            case State.Melee:   UpdateMelee(dist);  break;
            case State.Pattern: UpdatePattern();    break;
        }
    }

    private void UpdateWander(float dist)
    {
        if (dist <= detectionRange) { state = State.Chase; return; }
        Wander();
    }

    private void UpdateChase(float dist)
    {
        patternTimer += Time.deltaTime;

        if (patternTimer >= patternCooldown && CanStartPattern(dist))
        {
            EnterPattern();
            return;
        }

        if (dist <= meleeRange) { state = State.Melee; meleeTimer = 0f; return; }
        if (dist > detectionRange) { state = State.Wander; SetNewWanderTarget(); return; }
        MoveSmooth((player.position - transform.position).normalized * moveSpeed);
    }

    private void UpdateMelee(float dist)
    {
        if (dist > meleeRange) { HideMeleeIndicator(); state = State.Chase; return; }

        MoveSmooth(Vector2.zero);
        meleeTimer += Time.deltaTime;

        if (meleeTimer >= meleeInterval - 0.5f) ShowMeleeIndicator();

        if (meleeTimer >= meleeInterval)
        {
            DamagePlayer(statManager.getStat(StatType.DAMAGE).calibratedValue);
            meleeTimer = 0f;
            HideMeleeIndicator();
        }
    }

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
        state = State.Chase;
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        ApplyDeathRewards();
        HideMeleeIndicator();
        OnBossDeath();
        StartCoroutine(DieRoutine(0f));
    }

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

    internal void HideMeleeIndicator() { if (meleeIndicator != null) meleeIndicator.SetActive(false); }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue; Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;  Gizmos.DrawWireSphere(transform.position, meleeRange);
    }

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
        return activeTree == SkillTree.Wild ? CanStartWildPattern(dist)
                                           : CanStartMagicPattern(dist);
    }

    private void OnBossDeath()
    {
        if (activeTree == SkillTree.Wild) OnWildBossDeath();
        else                              OnMagicBossDeath();
    }
}