using UnityEngine;

public class DamageAction : ActiveAction
{
    public override void Process(GameObject player, ActiveSkillData data)
    {
        Debug.Log($"Executing {data.skillName}!");
    }
}
