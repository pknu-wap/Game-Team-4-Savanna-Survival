using UnityEngine;

public abstract class ActiveAction : ScriptableObject
{
    public virtual bool CanProcess(GameObject player, ActiveSkillData data) => true;
    public abstract void Process(GameObject player, ActiveSkillData data);
    public virtual void Clear(GameObject player) { }
}
