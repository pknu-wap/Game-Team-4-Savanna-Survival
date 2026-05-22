public abstract class StatusEffectBaseTemp
{
    public bool IsExpired { get; protected set; }

    public virtual void OnApply(Entity target)  { }
    public virtual void OnRemove(Entity target) { }
    public abstract void OnTick(Entity target, float deltaTime);
}