using UnityEngine;

/// 중독 상태이상.
/// tickInterval마다 최대 체력의 healthPercent% 데미지 적용.
/// 중첩 시 Refresh()로 지속 시간만 갱신.
public class PoisonEffect : StatusEffectBase
{
    private readonly float healthPercent;
    private readonly float tickInterval;
    private float duration;
    private float tickTimer;

    public PoisonEffect(float healthPercent, float tickInterval, float duration)
    {
        EffectName         = "Poison";
        this.healthPercent = healthPercent;
        this.tickInterval  = tickInterval;
        this.duration      = duration;
    }

    public void Refresh(float newDuration) => duration = newDuration;

    public override void OnTick(Entity target, float deltaTime)
    {
        duration  -= deltaTime;
        tickTimer += deltaTime;

        if (duration <= 0f) { IsExpired = true; return; }
        if (tickTimer < tickInterval) return;
        tickTimer = 0f;

        PlayerStatManager psm = target.GetComponent<PlayerStatManager>();
        float maxHp = psm != null ? psm.StatCore.getStat(StatType.MAX_HEALTH).rawValue : 1f;
        target.TakeDamage(maxHp * healthPercent);
    }
}