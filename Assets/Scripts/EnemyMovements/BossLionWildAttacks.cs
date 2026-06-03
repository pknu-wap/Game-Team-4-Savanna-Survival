using UnityEngine;

/// 야생 트리 패턴 실행 로직.
public partial class BossLion
{
    private GameObject w1Indicator;
    private GameObject w2Indicator;

    // ── W1 할퀴기 ────────────────────────────────────────────

    internal void EnterW1Telegraph()
    {
        wildPattern      = WildPattern.W1Telegraph;
        wildPatternTimer = 0f;
        MoveSmooth(Vector2.zero);
        ShowW1Indicator();
        Debug.Log("[BossLion-Wild] W1 전조 시작");
    }

    internal void UpdateW1Telegraph()
    {
        if (wildPatternTimer < w1Telegraph) return;
        HideW1Indicator();
        wildPatternTimer = 0f;
        wildPattern      = WildPattern.W1Swipe1;
    }

    internal void UpdateW1Swipe1()
    {
        ExecuteSwipe();
        Debug.Log("[BossLion-Wild] W1 1타");
        wildPattern      = WildPattern.W1Delay;
        wildPatternTimer = 0f;
    }

    internal void UpdateW1Delay()
    {
        if (wildPatternTimer < w1SwipeDelay) return;
        wildPatternTimer = 0f;
        wildPattern      = WildPattern.W1Swipe2;
    }

    internal void UpdateW1Swipe2()
    {
        ExecuteSwipe();
        Debug.Log("[BossLion-Wild] W1 2타");
        wildPattern = WildPattern.None;
        ExitPattern();
    }

    private void ExecuteSwipe()
    {
        if (player == null) return;
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > w1Range) return;

        // 바라보는 방향: 플레이어 방향 기준 (transform.right 대신)
        Vector2 toPlayer   = (player.position - transform.position).normalized;
        Vector2 facing     = transform.localScale.x < 0 ? Vector2.right : Vector2.left;
        float   angleDiff  = Vector2.Angle(facing, toPlayer);
        if (angleDiff > w1SwipeAngle * 0.5f) return;

        DamagePlayer(statManager.getStat(StatType.DAMAGE).calibratedValue);
        ApplyBleedToPlayer(w1BleedStacks, w1BleedDps, w1BleedDur);
        Debug.Log("[BossLion-Wild] W1 할퀴기 타격 + 출혈 3스택");
    }

    private void ApplyBleedToPlayer(int stacks, float dps, float duration)
    {
        Entity target = player?.GetComponent<PlayerEffectTemp>();
        if (target == null) return;
        if (target.HasEffect<BleedEffect>(out BleedEffect existing))
            existing.AddStacks(stacks, duration);
        else
            target.ApplyEffect(new BleedEffect(stacks, dps, duration));
    }

    private void ShowW1Indicator()
    {
        if (w1IndicatorPrefab == null) return;
        w1Indicator ??= Instantiate(w1IndicatorPrefab, transform);
        w1Indicator.transform.localPosition = Vector3.zero;
        w1Indicator.transform.localScale    = Vector3.one * w1Range * 2f;
        w1Indicator.SetActive(true);
    }

    internal void HideW1Indicator() { if (w1Indicator != null) w1Indicator.SetActive(false); }

    // ── W2 포효 ──────────────────────────────────────────────

    internal void EnterW2Telegraph()
    {
        wildPattern      = WildPattern.W2Telegraph;
        wildPatternTimer = 0f;
        MoveSmooth(Vector2.zero);
        ShowW2Indicator();
        Debug.Log("[BossLion-Wild] W2 포효 전조 시작");
    }

    internal void UpdateW2Telegraph()
    {
        if (wildPatternTimer < w2Telegraph) return;
        HideW2Indicator();
        wildPatternTimer = 0f;
        wildPattern      = WildPattern.W2Burst;
    }

    internal void UpdateW2Burst()
    {
        if (player == null) { ExitPattern(); return; }

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= w2Range)
        {
            DamagePlayer(w2Damage);
            ApplyStunToPlayer(w2StunDuration);
            Debug.Log($"[BossLion-Wild] W2 포효 명중 — 데미지 {w2Damage} / 스턴 {w2StunDuration}s");
        }
        else
        {
            Debug.Log("[BossLion-Wild] W2 포효 빗나감");
        }

        wildPattern = WildPattern.None;
        ExitPattern();
    }

    private void ApplyStunToPlayer(float duration)
    {
        PlayerEffectTemp target = player?.GetComponent<PlayerEffectTemp>();
        if (target == null) return;
        if (target.HasEffect<StunEffect>(out StunEffect existing))
            target.RemoveEffect(existing);
        target.ApplyEffect(new StunEffect(duration));
        Debug.Log($"[BossLion-Wild] 스턴 부여 — {duration}s");
    }

    private void ShowW2Indicator()
    {
        if (w2IndicatorPrefab == null) return;
        if (w2Indicator == null)
        {
            w2Indicator = Instantiate(w2IndicatorPrefab, transform);
            w2Indicator.transform.localPosition = Vector3.zero;
            w2Indicator.transform.localScale    = Vector3.one * w2Range * 2f;
        }
        w2Indicator.SetActive(true);
    }

    internal void HideW2Indicator() { if (w2Indicator != null) w2Indicator.SetActive(false); }
}