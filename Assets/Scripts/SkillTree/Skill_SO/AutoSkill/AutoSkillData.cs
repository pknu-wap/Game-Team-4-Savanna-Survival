using UnityEngine;

[CreateAssetMenu(fileName = "NewAutoSkill", menuName = "SkillTree/AutoSkillData")]
public class AutoSkillData : BaseSkillData
{
    public float interval;
    public float range;
    public AutoAction action;
}
