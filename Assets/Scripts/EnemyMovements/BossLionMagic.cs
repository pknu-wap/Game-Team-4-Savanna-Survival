using System.Collections.Generic;
using UnityEngine;

/// 마법 트리 — 상태머신, 초기화, 패턴 선택.
/// 패턴 실행 로직은 BossLionMagicAttacks.cs(partial)에 분리.
public partial class BossLion
{
    [Header("Magic - ArcaneCircle (M1)")]
    [SerializeField] internal float      m1Radius          = 1.5f;
    [SerializeField] internal float      m1Delay           = 0.8f;
    [SerializeField] internal float      m1FuseTime        = 1.5f;
    [SerializeField] internal float      m1Damage          = 35f;
    [SerializeField] internal int        m1Count           = 3;
    [SerializeField] internal GameObject m1IndicatorPrefab;
    [SerializeField] internal GameObject m1ExplosionPrefab;

    [Header("Magic - FinalJudgment (M2)")]
    [SerializeField] internal float      m2HpThreshold     = 0.5f;
    [SerializeField] internal float      m2DamageRatio     = 0.7f;
    [SerializeField] internal float      m2Telegraph       = 2.5f;
    [SerializeField] internal int        m2ZoneCount       = 12;
    [SerializeField] internal float      m2ZoneRadius      = 2.0f;
    [SerializeField] internal int        m2SafeZoneCount   = 3;
    [SerializeField] internal float      m2MapRadius       = 12f;
    [SerializeField] internal GameObject m2IndicatorPrefab;

    [Header("Magic - Cooldown")]
    [SerializeField] private float magicBaseCooldown = 8f;

    internal enum MagicPattern { None, M1Spawning, M1Waiting, M2Telegraph, M2Strike }
    internal MagicPattern magicPattern      = MagicPattern.None;
    internal float        magicPatternTimer;
    internal bool         m2Used;

    internal int              m1SpawnedCount;
    internal float            m1SpawnTimer;
    internal List<GameObject> m1ActiveIndicators = new();
    internal List<Vector2>    m1Positions        = new();
    internal List<GameObject> m2Indicators       = new();
    internal List<Vector2>    m2HitPositions     = new();

    void InitMagicCooldowns()
    {
        patternCooldown = magicBaseCooldown;
    }

    bool CanStartMagicPattern(float dist)
    {
        return dist <= detectionRange;
    }

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

        activePatternId = 0;
        EnterM1Spawning();
    }

    void UpdateMagicPattern()
    {
        magicPatternTimer += Time.deltaTime;

        switch (magicPattern)
        {
            case MagicPattern.M1Spawning:  UpdateM1Spawning();  break;
            case MagicPattern.M1Waiting:   UpdateM1Waiting();   break;
            case MagicPattern.M2Telegraph: UpdateM2Telegraph(); break;
            case MagicPattern.M2Strike:    UpdateM2Strike();    break;
        }
    }

    void OnMagicBossDeath()
    {
        foreach (GameObject ind in m1ActiveIndicators) if (ind != null) Destroy(ind);
        foreach (GameObject ind in m2Indicators)       if (ind != null) Destroy(ind);
    }
}