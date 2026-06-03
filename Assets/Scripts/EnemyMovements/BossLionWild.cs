using UnityEngine;

/// 야생 트리 — 상태머신, 초기화, 패턴 선택.
/// 패턴 실행 로직은 BossLionWildAttacks.cs(partial)에 분리.
public partial class BossLion
{
    [Header("Wild - ClawSweep (W1)")]
    [SerializeField] internal float      w1Telegraph   = 2.0f;
    [SerializeField] internal float      w1SwipeAngle  = 120f;
    [SerializeField] internal float      w1Range       = 3.5f;
    [SerializeField] internal float      w1SwipeDelay  = 0.4f;
    [SerializeField] internal int        w1BleedStacks = 3;
    [SerializeField] internal float      w1BleedDps    = 4f;
    [SerializeField] internal float      w1BleedDur    = 5f;
    [SerializeField] internal GameObject w1IndicatorPrefab;

    [Header("Wild - Roar (W2)")]
    [SerializeField] internal float      w2Range        = 4f;
    [SerializeField] internal float      w2Damage       = 50f;
    [SerializeField] internal float      w2StunDuration = 2f;
    [SerializeField] internal float      w2Telegraph    = 1.0f;
    [SerializeField] internal GameObject w2IndicatorPrefab;

    [Header("Wild - Cooldown")]
    [SerializeField] private float wildBaseCooldown = 7f;

    internal enum WildPattern { None, W1Telegraph, W1Swipe1, W1Delay, W1Swipe2, W2Telegraph, W2Burst }
    internal WildPattern wildPattern      = WildPattern.None;
    internal float       wildPatternTimer;
    internal int         lastWildPatternId = -1;

    void InitWildCooldowns()
    {
        patternCooldown = wildBaseCooldown;
    }

    bool CanStartWildPattern(float dist)
    {
        return dist <= detectionRange;
    }

    void StartNextWildPattern()
    {

        int next;
        if (lastWildPatternId == 0)      next = 1;
        else if (lastWildPatternId == 1) next = 0;
        else                             next = Random.Range(0, 2);

        activePatternId   = next;
        lastWildPatternId = next;
        wildPatternTimer  = 0f;

        switch (activePatternId)
        {
            case 0: EnterW1Telegraph(); break;
            case 1: EnterW2Telegraph(); break;
        }
    }

    void UpdateWildPattern()
    {
        wildPatternTimer += Time.deltaTime;

        switch (wildPattern)
        {
            case WildPattern.W1Telegraph: UpdateW1Telegraph(); break;
            case WildPattern.W1Swipe1:   UpdateW1Swipe1();    break;
            case WildPattern.W1Delay:    UpdateW1Delay();     break;
            case WildPattern.W1Swipe2:   UpdateW1Swipe2();    break;
            case WildPattern.W2Telegraph:UpdateW2Telegraph(); break;
            case WildPattern.W2Burst:    UpdateW2Burst();     break;
        }
    }

    void OnWildBossDeath()
    {
        HideW1Indicator();
        HideW2Indicator();
    }
}