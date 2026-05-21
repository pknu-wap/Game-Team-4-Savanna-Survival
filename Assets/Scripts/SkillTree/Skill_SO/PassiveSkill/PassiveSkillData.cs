using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPassiveSkill", menuName = "SkillTree/PassiveSkillData")]
public class PassiveSkillData : BaseSkillData
{
    public List<PassiveEffect> effects = new();
}
