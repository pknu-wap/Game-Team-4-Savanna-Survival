using UnityEngine;

public enum SkillType
{
    Fireball,
    Dash,
    Roar
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public float damage;
    public float cooldown;
    public string description;
    public SkillType skillType;
}