using UnityEngine;
using System.Collections.Generic;

public enum SkillTreeType
{
    None,
    Wildness,
    Fantasy
}

public enum SkillUnlockBehavior
{
    AddIndependent,
    ReplaceGroup,
    ModifyParent
}

public abstract class BaseSkillData : ScriptableObject
{
    public string id;
    public string skillName;
    public Sprite icon;
    [TextArea(3, 5)]
    public string description;
    public int cost;
    public List<BaseSkillData> prerequisites = new();
    public Vector2 treePosition;

    public SkillTreeType treeType;
    public SkillUnlockBehavior unlockBehavior;
    public string skillGroupId;
    public int skillLevel;
}
