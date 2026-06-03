using System;

/// 상태이상 이벤트 버스.
/// UI, 패시브 스킬 등이 구독해 상태이상 변화를 감지합니다.
public static class EffectEventBus
{
    /// 상태이상이 적용될 때 (대상, 효과)
    public static event Action<Entity, StatusEffectBase> OnEffectApplied;
    /// 상태이상이 제거될 때 (대상, 효과)
    public static event Action<Entity, StatusEffectBase> OnEffectRemoved;
    /// 상태이상 틱이 발생할 때 (대상, 효과)
    public static event Action<Entity, StatusEffectBase> OnEffectTicked;

    public static void PublishApplied(Entity target, StatusEffectBase effect) => OnEffectApplied?.Invoke(target, effect);
    public static void PublishRemoved(Entity target, StatusEffectBase effect) => OnEffectRemoved?.Invoke(target, effect);
    public static void PublishTicked(Entity target, StatusEffectBase effect)  => OnEffectTicked?.Invoke(target, effect);
}
