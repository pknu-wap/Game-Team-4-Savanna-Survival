using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public abstract void TakeDamage(float damage);

    /// 스턴 상태 여부. Enemy 이동, PlayerMovement에서 참조합니다.
    public bool IsStunned { get; private set; }

    private readonly List<StatusEffectBase> activeEffects = new();

    public void ApplyEffect(StatusEffectBase effect)
    {
        effect.OnApply(this);
        activeEffects.Add(effect);
        if (effect is StunEffect) IsStunned = true;
        EffectEventBus.PublishApplied(this, effect);
    }

    public void RemoveEffect(StatusEffectBase effect)
    {
        effect.OnRemove(this);
        activeEffects.Remove(effect);
        if (effect is StunEffect) RefreshStunState();
        EffectEventBus.PublishRemoved(this, effect);
    }

    public bool HasEffect<T>() where T : StatusEffectBase
    {
        foreach (StatusEffectBase effect in activeEffects)
            if (effect is T) return true;
        return false;
    }

    public bool HasEffect<T>(out T found) where T : StatusEffectBase
    {
        foreach (StatusEffectBase effect in activeEffects)
        {
            if (effect is T t) { found = t; return true; }
        }
        found = null;
        return false;
    }

    protected void TickEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].OnTick(this, Time.deltaTime);
            EffectEventBus.PublishTicked(this, activeEffects[i]);

            if (activeEffects[i].IsExpired)
            {
                StatusEffectBase expired = activeEffects[i];
                expired.OnRemove(this);
                activeEffects.RemoveAt(i);
                if (expired is StunEffect) RefreshStunState();
                EffectEventBus.PublishRemoved(this, expired);
            }
        }
    }

    private void RefreshStunState()
    {
        IsStunned = HasEffect<StunEffect>();
    }
}