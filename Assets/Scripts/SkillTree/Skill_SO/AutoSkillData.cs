using UnityEngine;

[CreateAssetMenu(fileName = "NewAutoSkill", menuName = "SkillTree/AutoSkillData")]
public class AutoSkillData : BaseSkillData
{
    public float interval;
    public float range;
    public bool requiresTarget = true;
    
public AutoAction action;
}
