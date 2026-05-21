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