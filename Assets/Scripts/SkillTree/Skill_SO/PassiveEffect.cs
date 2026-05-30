using UnityEngine;

public abstract class PassiveEffect : ScriptableObject
{
    public abstract void Apply(GameObject player);
    public virtual void Remove(GameObject player) { }
}
