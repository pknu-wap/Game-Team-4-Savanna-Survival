/// 상태이상 베이스 클래스.
/// Temp 접미사 제거 및 구조 개편.
public abstract class StatusEffectBase
{
    public bool   IsExpired  { get; protected set; }
    public string EffectName { get; protected set; } = "Unknown";

    public virtual void OnApply(Entity target)  { }
    public virtual void OnRemove(Entity target) { }
    public abstract void OnTick(Entity target, float deltaTime);
}
