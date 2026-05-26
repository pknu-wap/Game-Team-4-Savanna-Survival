using UnityEngine;

public abstract class ActiveAction : ScriptableObject
{
    public abstract void Process(GameObject player, ActiveSkillData data);
    public virtual void Clear(GameObject player) { }
}
