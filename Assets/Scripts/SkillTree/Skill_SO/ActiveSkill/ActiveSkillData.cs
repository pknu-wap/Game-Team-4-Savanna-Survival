using UnityEngine;

[CreateAssetMenu(fileName = "NewActiveSkill", menuName = "SkillTree/ActiveSkillData")]
public class ActiveSkillData : BaseSkillData
{
    public float cooldown;
    public ActiveAction action;
}
