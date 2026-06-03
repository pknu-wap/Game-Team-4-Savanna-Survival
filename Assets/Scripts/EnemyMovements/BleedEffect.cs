using UnityEngine;

/// 출혈 상태이상.
/// 스택 1개당 dotDamagePerStack/초 데미지를 tickInterval마다 적용.
/// 중첩 시 AddStacks()로 스택 누적 + 지속 시간 갱신.
public class BleedEffect : StatusEffectBase
{
    public int Stacks { get; private set; }

    private readonly float dotDamagePerStack;
    private readonly float tickInterval;
    private float duration;
    private float tickTimer;

    public BleedEffect(int stacks, float dotDamagePerStack, float duration, float tickInterval = 0.5f)
    {
        EffectName           = "Bleed";
        Stacks               = stacks;
        this.dotDamagePerStack = dotDamagePerStack;
        this.duration        = duration;
        this.tickInterval    = tickInterval;
    }

    public void AddStacks(int count, float newDuration)
    {
        Stacks   += count;
        duration  = newDuration;
    }

    public override void OnTick(Entity target, float deltaTime)
    {
        duration  -= deltaTime;
        tickTimer += deltaTime;

        if (duration <= 0f) { IsExpired = true; return; }

        if (tickTimer >= tickInterval)
        {
            target.TakeDamage(dotDamagePerStack * Stacks * tickInterval);
            tickTimer = 0f;
        }
    }
}