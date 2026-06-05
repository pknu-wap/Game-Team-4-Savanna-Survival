using UnityEngine;

public static class AnimatorSafe
{
    public static void SetTrigger(Animator anim, string name)
    {
        if (anim == null || !HasParam(anim, name, AnimatorControllerParameterType.Trigger)) return;
        anim.SetTrigger(name);
    }

    public static void SetBool(Animator anim, string name, bool value)
    {
        if (anim == null || !HasParam(anim, name, AnimatorControllerParameterType.Bool)) return;
        anim.SetBool(name, value);
    }

    public static void SetBool(Animator anim, int hash, bool value)
    {
        if (anim == null || !HasParamHash(anim, hash, AnimatorControllerParameterType.Bool)) return;
        anim.SetBool(hash, value);
    }

    public static void SetFloat(Animator anim, string name, float value)
    {
        if (anim == null || !HasParam(anim, name, AnimatorControllerParameterType.Float)) return;
        anim.SetFloat(name, value);
    }

    public static void SetFloat(Animator anim, int hash, float value)
    {
        if (anim == null || !HasParamHash(anim, hash, AnimatorControllerParameterType.Float)) return;
        anim.SetFloat(hash, value);
    }

    static bool HasParam(Animator anim, string name, AnimatorControllerParameterType type)
    {
        foreach (var p in anim.parameters)
            if (p.name == name && p.type == type) return true;
        return false;
    }

    static bool HasParamHash(Animator anim, int hash, AnimatorControllerParameterType type)
    {
        foreach (var p in anim.parameters)
            if (p.nameHash == hash && p.type == type) return true;
        return false;
    }
}