using UnityEngine;

public class PoisonEffectTemp : StatusEffectBaseTemp
{
    private readonly float healthPercent;
    private readonly float tickInterval;
    private float duration;
    private float tickTimer;

    public PoisonEffectTemp(float healthPercent, float tickInterval, float duration)
    {
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