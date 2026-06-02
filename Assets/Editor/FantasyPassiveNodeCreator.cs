using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Fantasy 패시브 스킬 노드들을 현재 열린 씬의 SkillContainer(Fantasy 페이지)에 일괄 등록한다.
/// 메뉴: Tools > SkillTree > Register Fantasy Passive Nodes
/// </summary>
public static class FantasyPassiveNodeCreator
{
    private const string PrefabPath = "Assets/Prefabs/SkillNode.prefab";

    // 등록할 Fantasy 패시브 스킬 에셋 경로
    private static readonly string[] SkillPaths =
    {
        "Assets/ScriptableObjects/Skills/Fantasy/Skill_MagicFocus.asset",
        "Assets/ScriptableObjects/Skills/Fantasy/Skill_MagicOverload.asset",
        "Assets/ScriptableObjects/Skills/Fantasy/Skill_PredationMagic.asset",
    };

    [MenuItem("Tools/SkillTree/Register Fantasy Passive Nodes")]
    public static void RegisterNodes()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[FantasyNodes] SkillNode 프리팹을 찾을 수 없습니다: {PrefabPath}");
            return;
        }

        SkillTreeWindow window = Object.FindObjectOfType<SkillTreeWindow>(true);
        if (window == null)
        {
            Debug.LogError("[FantasyNodes] 현재 씬에 SkillTreeWindow가 없습니다. 스킬 트리가 있는 씬을 열어주세요.");
            return;
        }

        Transform container = window.transform.Find("SkillContainer");
        if (container == null)
        {
            Debug.LogError("[FantasyNodes] SkillTreeWindow 아래 SkillContainer를 찾을 수 없습니다.");
            return;
        }

        // Fantasy 트리 = 마지막 페이지로 가정 (없으면 컨테이너 직속)
        Transform page = container.childCount > 0
            ? container.GetChild(container.childCount - 1)
            : container;

        int added = 0, skipped = 0;

        foreach (string path in SkillPaths)
        {
            BaseSkillData skill = AssetDatabase.LoadAssetAtPath<BaseSkillData>(path);
            if (skill == null)
            {
                Debug.LogWarning($"[FantasyNodes] 스킬 에셋을 찾을 수 없음: {path}");
                continue;
            }

            // 중복 방지
            bool exists = false;
            foreach (var existing in page.GetComponentsInChildren<SkillTreeNodeUI>(true))
            {
                if (existing.SkillData == skill) { exists = true; break; }
            }
            if (exists)
            {
                Debug.Log($"[FantasyNodes] 이미 등록됨, 건너뜀: {skill.skillName}");
                skipped++;
                continue;
            }

            GameObject node = (GameObject)PrefabUtility.InstantiatePrefab(prefab, page);
            node.name = skill.skillName;

            SkillTreeNodeUI nodeUI = node.GetComponent<SkillTreeNodeUI>();
            SerializedObject so = new SerializedObject(nodeUI);
            SerializedProperty prop = so.FindProperty("skillData");
            if (prop != null)
            {
                prop.objectReferenceValue = skill;
                so.ApplyModifiedProperties();
            }

            RectTransform rt = node.GetComponent<RectTransform>();
            if (rt != null)
                rt.anchoredPosition = skill.treePosition;

            Undo.RegisterCreatedObjectUndo(node, "Register Fantasy Passive Node");
            added++;
            Debug.Log($"[FantasyNodes] 등록: {skill.skillName} @ {skill.treePosition}");
        }

        if (added > 0)
            EditorSceneManager.MarkSceneDirty(window.gameObject.scene);

        Debug.Log($"[FantasyNodes] 완료 — 추가 {added}개, 건너뜀 {skipped}개. (페이지: {page.name})");
    }
}
