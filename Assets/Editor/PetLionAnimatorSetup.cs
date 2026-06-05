using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class PetLionAnimatorSetup
{
    [MenuItem("Tools/Pets/Create Lion Animator Override Controllers")]
    public static void CreateOverrideControllers()
    {
        var baseCtrl = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
            "Assets/Animations/Pets/PetLionBase.controller");

        if (baseCtrl == null)
        {
            Debug.LogError("[PetLion] PetLionBase.controller 를 찾을 수 없습니다.");
            return;
        }

        CreateOverride(baseCtrl, "BabyLion");
        CreateOverride(baseCtrl, "AdultLion");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[PetLion] BabyLionAnimator.overrideController + AdultLionAnimator.overrideController 생성 완료");
    }

    private static void CreateOverride(RuntimeAnimatorController baseCtrl, string lionName)
    {
        string folder = $"Assets/Animations/Pets/{lionName}";
        string outPath = $"{folder}/{lionName}Animator.overrideController";

        var walk   = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{folder}/{lionName}_Walk.anim");
        var attack = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{folder}/{lionName}_Attack.anim");

        var oc = new AnimatorOverrideController(baseCtrl);
        oc.name = lionName + "Animator";

        var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        oc.GetOverrides(pairs);

        for (int i = 0; i < pairs.Count; i++)
        {
            AnimationClip orig = pairs[i].Key;
            if (orig == null) continue;
            if (orig.name.EndsWith("_Walk"))
                pairs[i] = new KeyValuePair<AnimationClip, AnimationClip>(orig, walk);
            else if (orig.name.EndsWith("_Attack"))
                pairs[i] = new KeyValuePair<AnimationClip, AnimationClip>(orig, attack);
        }

        oc.ApplyOverrides(pairs);

        // 기존 파일 있으면 삭제 후 재생성
        if (AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(outPath) != null)
            AssetDatabase.DeleteAsset(outPath);

        AssetDatabase.CreateAsset(oc, outPath);
        Debug.Log($"[PetLion] {outPath} 생성");
    }
}
