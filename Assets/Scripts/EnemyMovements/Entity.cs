using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public abstract void TakeDamage(float damage);

    private readonly List<StatusEffectBaseTemp> activeEffects = new();

    public void ApplyEffect(StatusEffectBaseTemp effect)
    {
        effect.OnApply(this);
        activeEffects.Add(effect);
    }

    public void RemoveEffect(StatusEffectBaseTemp effect)
    {
        effect.OnRemove(this);
        activeEffects.Remove(effect);
    }

    public bool HasEffect<T>() where T : StatusEffectBaseTemp
    {
        foreach (StatusEffectBaseTemp effect in activeEffects)
            if (effect is T) return true;
        return false;
    }

    // 이미 걸린 효과를 꺼내 갱신할 때 사용 (PoisonEffectTemp.Refresh 등)
    public bool HasEffect<T>(out T found) where T : StatusEffectBaseTemp
    {
        foreach (StatusEffectBaseTemp effect in activeEffects)
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
            if (activeEffects[i].IsExpired)
            {
                activeEffects[i].OnRemove(this);
                activeEffects.RemoveAt(i);
            }
        }
    }
}