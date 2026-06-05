using System.Collections.Generic;
using UnityEngine;

/// 마법 트리 — 상태머신, 초기화, 패턴 선택.
/// 패턴 실행 로직은 BossLionMagicAttacks.cs(partial)에 분리.
public partial class BossLion
{
    [Header("Magic - ArcaneCircle (M1)")]
    [Tooltip("마법진 하나의 반경")]
    [SerializeField] internal float      m1Radius      = 1.5f;
    [Tooltip("마법진 생성 후 폭발까지의 전조 시간")]
    [SerializeField] internal float      m1FuseTime    = 0.6f;
    [Tooltip("총 폭발 횟수")]
    [SerializeField] internal int        m1Count       = 4;
    [Tooltip("폭발 간격 (다음 마법진 생성까지 대기)")]
    [SerializeField] internal float      m1Interval    = 0.3f;
    [SerializeField] internal float      m1Damage      = 35f;
    [SerializeField] internal GameObject m1IndicatorPrefab;
    [SerializeField] internal GameObject m1ExplosionPrefab;

    [Header("Magic - FinalJudgment (M2)")]
    [SerializeField] internal float      m2HpThreshold   = 0.5f;
    [SerializeField] internal float      m2DamageRatio   = 0.7f;
    [SerializeField] internal float      m2Telegraph     = 2.5f;
    [SerializeField] internal int        m2ZoneCount     = 12;
    [SerializeField] internal float      m2ZoneRadius    = 2.0f;
    [SerializeField] internal int        m2SafeZoneCount = 3;
    [SerializeField] internal float      m2MapRadius     = 12f;
    [Tooltip("피격 구역 인디케이터 프리팹 (위험 색상)")]
    [SerializeField] internal GameObject m2DangerIndicatorPrefab;
    [Tooltip("안전 구역 인디케이터 프리팹 (안전 색상 — 별도 프리팹)")]
    [SerializeField] internal GameObject m2SafeIndicatorPrefab;

    [Header("Magic - Skill Patterns")]
    [SerializeField] [Range(0f, 1f)] private float magicSkillPatternChance = 0.4f;

    [Header("Magic - Cooldown")]
    [SerializeField] private float magicBaseCooldown = 8f;

    // ── M1 상태 ──────────────────────────────────────────────
    // 단순화: Spawning(전조) → Burst(폭발) → 다시 Spawning 반복
    internal enum MagicPattern { None, M1Fuse, M1Interval, M2Telegraph, M2Strike, Skill }
    internal MagicPattern magicPattern      = MagicPattern.None;
    internal float        magicPatternTimer;
    internal bool         m2Used;

    // M1
    internal int        m1FiredCount;       // 지금까지 폭발한 횟수
    internal GameObject m1CurrentIndicator; // 현재 활성 마법진 인디케이터

    // M2
    internal List<GameObject> m2DangerIndicators = new();
    internal List<GameObject> m2SafeIndicators   = new();
    internal List<Vector2>    m2HitPositions      = new();
    internal List<Vector2>    m2SafePositions     = new();

    void InitMagicCooldowns() => patternCooldown = magicBaseCooldown;

    bool CanStartMagicPattern(float dist) => true;

    void StartNextMagicPattern()
    {
        magicPatternTimer = 0f;

        float hpRatio = currentHp / statManager.getStat(StatType.HEALTH).rawValue;
        if (!m2Used && hpRatio < m2HpThreshold)
        {
            activePatternId = 1;
            EnterM2Telegraph();
            return;
        }

        if (HasSkillReady() && Random.value < magicSkillPatternChance)
        {
            activePatternId = 2;
            EnterMagicSkillPattern();
            return;
        }

        activePatternId = 0;
        EnterM1Pattern();
    }

    void UpdateMagicPattern()
    {
        magicPatternTimer += Time.deltaTime;

        switch (magicPattern)
        {
            case MagicPattern.M1Fuse:      UpdateM1Fuse();          break;
            case MagicPattern.M1Interval:  UpdateM1Interval();      break;
            case MagicPattern.M2Telegraph: UpdateM2Telegraph();     break;
            case MagicPattern.M2Strike:    UpdateM2Strike();        break;
            case MagicPattern.Skill:       UpdateMagicSkillPattern(); break;
        }
    }

    // ── 스킬 패턴 ────────────────────────────────────────────

    private void EnterMagicSkillPattern()
    {
        magicPattern      = MagicPattern.Skill;
        magicPatternTimer = 0f;
        MoveSmooth(Vector2.zero);
    }

    private void UpdateMagicSkillPattern()
    {
        bool executed = TryExecuteNextSkill();
        if (!executed)
        {
            Debug.LogWarning("[BossLion-Magic] 준비된 스킬 슬롯 없음 — M1으로 전환");
            EnterM1Pattern();
            return;
        }
        magicPattern = MagicPattern.None;
        ExitPattern();
    }

    void OnMagicBossDeath()
    {
        if (m1CurrentIndicator != null) Destroy(m1CurrentIndicator);
        foreach (var ind in m2DangerIndicators) if (ind != null) Destroy(ind);
        foreach (var ind in m2SafeIndicators)   if (ind != null) Destroy(ind);
    }
}