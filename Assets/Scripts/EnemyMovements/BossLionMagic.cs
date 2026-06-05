using System.Collections.Generic;
using UnityEngine;

/// 마법 트리 — 상태머신, 초기화, 패턴 선택.
public partial class BossLion
{
    [Header("Magic - ArcaneCircle (M1)")]
    [SerializeField] internal float      m1Radius      = 1.5f;
    [SerializeField] internal float      m1FuseTime    = 0.6f;
    [SerializeField] internal int        m1Count       = 4;
    [SerializeField] internal float      m1Interval    = 0.3f;
    [SerializeField] internal float      m1Damage      = 35f;
    [SerializeField] internal GameObject m1IndicatorPrefab;
    [SerializeField] internal GameObject m1ExplosionPrefab;

    [Header("Magic - FinalJudgment (M2)")]
    [Tooltip("HP 비율이 이 값 미만이 되면 즉시 M2 발동 (0.6 = 60%)")]
    [SerializeField] internal float      m2HpThreshold   = 0.6f;
    [Tooltip("맞은 경우 플레이어 최대 HP 대비 피해 비율")]
    [SerializeField] internal float      m2DamageRatio   = 0.7f;
    [Tooltip("레이저 총 발사 횟수")]
    [SerializeField] internal int        m2LaserCount    = 8;
    [Tooltip("보스 기준 레이저 최소 거리")]
    [SerializeField] internal float      m2MinRadius     = 1.5f;
    [Tooltip("보스 기준 레이저 최대 거리")]
    [SerializeField] internal float      m2MaxRadius     = 5f;
    [Tooltip("레이저 하나당 전조 시간(초)")]
    [SerializeField] internal float      m2LaserTelegraph = 0.8f;
    [Tooltip("레이저 발사 후 다음 레이저까지 간격(초)")]
    [SerializeField] internal float      m2LaserInterval  = 0.3f;
    [Tooltip("레이저 판정 반경")]
    [SerializeField] internal float      m2ZoneRadius    = 1.2f;
    [SerializeField] internal GameObject m2DangerIndicatorPrefab;

    [Header("Magic - Skill Patterns")]
    [SerializeField] [Range(0f, 1f)] private float magicSkillPatternChance = 0.4f;

    [Header("Magic - Cooldown")]
    [SerializeField] private float magicBaseCooldown = 8f;

    internal enum MagicPattern { None, M1Fuse, M1Interval, M2Sequence, Skill }
    internal MagicPattern magicPattern      = MagicPattern.None;
    internal float        magicPatternTimer;
    internal bool         m2Used;

    // M1
    internal int        m1FiredCount;
    internal GameObject m1CurrentIndicator;

    // M2
    internal int        m2CurrentLaserIndex;
    internal bool       m2PlayerHitThisPattern;
    internal GameObject m2CurrentLaserIndicator;
    internal Vector2    m2CurrentLaserPos;

    internal enum M2Phase { Telegraph, Interval }
    internal M2Phase m2Phase;

    void InitMagicCooldowns() => patternCooldown = magicBaseCooldown;

    bool CanStartMagicPattern(float dist) => true;

    void StartNextMagicPattern()
    {
        magicPatternTimer = 0f;

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
            case MagicPattern.M1Fuse:     UpdateM1Fuse();          break;
            case MagicPattern.M1Interval: UpdateM1Interval();      break;
            case MagicPattern.M2Sequence: UpdateM2Sequence();      break;
            case MagicPattern.Skill:      UpdateMagicSkillPattern(); break;
        }
    }

    /// HP 임계값 감시 — BossLion.Move() 첫머리에서 매 프레임 호출됩니다.
    internal void CheckMagicInterrupt()
    {
        if (m2Used) return;
        float hpRatio = currentHp / statManager.getStat(StatType.HEALTH).rawValue;
        if (hpRatio >= m2HpThreshold) return;

        // 진행 중인 M1 인디케이터 즉시 정리
        if (m1CurrentIndicator != null)
        {
            m1CurrentIndicator.SetActive(false);
            Destroy(m1CurrentIndicator);
            m1CurrentIndicator = null;
        }
        if (m2CurrentLaserIndicator != null)
        {
            m2CurrentLaserIndicator.SetActive(false);
            Destroy(m2CurrentLaserIndicator);
            m2CurrentLaserIndicator = null;
        }

        // 상태 강제 전환
        state           = State.Pattern;
        activePatternId = 1;
        patternTimer    = 0f;
        MoveSmooth(Vector2.zero);
        EnterM2Sequence();
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
        if (m2CurrentLaserIndicator != null) Destroy(m2CurrentLaserIndicator);
    }
}