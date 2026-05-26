using UnityEngine;

public abstract class AutoAction : ScriptableObject
{
    public abstract void Process(GameObject player, AutoSkillData data);
    public virtual void Clear(GameObject player) { }
}
