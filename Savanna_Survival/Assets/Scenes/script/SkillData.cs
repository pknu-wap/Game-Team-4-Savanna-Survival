using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public float damage;
    public float cooldown;
    public string description;
}