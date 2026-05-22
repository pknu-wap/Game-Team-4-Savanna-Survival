// 출혈 상태이상.
// 스택 1개당 dotDamage/초 데미지를 tickInterval마다 적용한다.
// 동일 스택 수의 출혈이 중첩되면 지속 시간이 갱신된다.
using UnityEngine;

public class BleedEffectTemp : StatusEffectBaseTemp
{
    public int   Stacks      { get; private set; }

    private readonly float dotDamagePerStack;
    private readonly float tickInterval;
    private float duration;
    private float tickTimer;

    public BleedEffectTemp(int stacks, float dotDamagePerStack, float duration, float tickInterval = 0.5f)
    {
        Stacks                  = stacks;
        this.dotDamagePerStack  = dotDamagePerStack;
        this.duration           = duration;
        this.tickInterval       = tickInterval;
    }

    // 이미 출혈이 걸린 대상에게 추가 스택을 쌓을 때 호출한다.
    public void AddStacks(int count, float newDuration)
    {
        Stacks   += count;
        duration  = newDuration;
        Debug.Log($"[출혈] 스택 추가 → 현재 {Stacks}스택 / 지속 시간 {newDuration:F1}s 갱신");
    }

    public override void OnTick(Entity target, float deltaTime)
    {
        duration  -= deltaTime;
        tickTimer += deltaTime;

        if (duration <= 0f) { IsExpired = true; return; }

        if (tickTimer >= tickInterval)
        {
            float totalDamage = dotDamagePerStack * Stacks * tickInterval;
            target.TakeDamage(totalDamage);
            tickTimer = 0f;
            Debug.Log($"[출혈] 틱 데미지 {totalDamage:F1} / 현재 {Stacks}스택 / 남은 시간 {duration:F1}s");
        }
    }
}