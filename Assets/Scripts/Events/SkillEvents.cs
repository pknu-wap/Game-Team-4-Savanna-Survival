using System;

public static class SkillEvents
{
    public static event Action<BaseSkillData> OnSkillUse;

    public static void PublishSkillUse(BaseSkillData skill)
    {
        OnSkillUse?.Invoke(skill);
    }
}
