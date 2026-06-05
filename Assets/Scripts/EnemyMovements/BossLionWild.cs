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

    [Header("Wild - Skill Patterns")]
    [Tooltip("스킬 패턴(W_Skill)이 고유 패턴(W1/W2) 사이에 끼어드는 확률 (0~1)")]
    [SerializeField] [Range(0f, 1f)] private float wildSkillPatternChance = 0.4f;

    [Header("Wild - Cooldown")]
    [SerializeField] private float wildBaseCooldown = 7f;

    internal enum WildPattern { None, W1Telegraph, W1Swipe1, W1Delay, W1Swipe2, W2Telegraph, W2Burst, Skill }
    internal WildPattern wildPattern      = WildPattern.None;
    internal float       wildPatternTimer;
    internal int         lastWildPatternId = -1;

    void InitWildCooldowns()
    {
        patternCooldown = wildBaseCooldown;
    }

    bool CanStartWildPattern(float dist) => true;

    void StartNextWildPattern()
    {
        wildPatternTimer = 0f;

        // 스킬이 준비되어 있고 확률 조건을 만족하면 스킬 패턴 우선
        if (HasSkillReady() && Random.value < wildSkillPatternChance)
        {
            activePatternId   = 2; // 스킬 패턴
            lastWildPatternId = 2;
            EnterWildSkillPattern();
            return;
        }

        // W1(0) / W2(1) 교번 — 직전과 겹치지 않게
        int next;
        if      (lastWildPatternId == 0) next = 1;
        else if (lastWildPatternId == 1) next = 0;
        else                             next = Random.Range(0, 2);

        activePatternId   = next;
        lastWildPatternId = next;

        switch (next)
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
            case WildPattern.W1Telegraph: UpdateW1Telegraph();     break;
            case WildPattern.W1Swipe1:   UpdateW1Swipe1();        break;
            case WildPattern.W1Delay:    UpdateW1Delay();         break;
            case WildPattern.W1Swipe2:   UpdateW1Swipe2();        break;
            case WildPattern.W2Telegraph:UpdateW2Telegraph();     break;
            case WildPattern.W2Burst:    UpdateW2Burst();         break;
            case WildPattern.Skill:      UpdateWildSkillPattern(); break;
        }
    }

    // ── 스킬 패턴 ────────────────────────────────────────────

    private void EnterWildSkillPattern()
    {
        wildPattern      = WildPattern.Skill;
        wildPatternTimer = 0f;
        MoveSmooth(Vector2.zero);
        Debug.Log("[BossLion-Wild] 스킬 패턴 시작");
    }

    private void UpdateWildSkillPattern()
    {
        bool executed = TryExecuteNextSkill();
        if (!executed)
        {
            // 스킬이 없거나 쿨다운 중이면 W1/W2 고유 패턴으로 폴백
            Debug.LogWarning("[BossLion-Wild] 준비된 스킬 슬롯 없음 — 고유 패턴으로 전환");
            lastWildPatternId = 2; // 스킬 패턴 ID로 처리해 W1→W2 교번 유지
            int fallback = Random.Range(0, 2);
            lastWildPatternId = fallback;
            switch (fallback)
            {
                case 0: EnterW1Telegraph(); return;
                case 1: EnterW2Telegraph(); return;
            }
        }

        wildPattern = WildPattern.None;
        ExitPattern();
    }

    void OnWildBossDeath()
    {
        HideW1Indicator();
        HideW2Indicator();
    }
}