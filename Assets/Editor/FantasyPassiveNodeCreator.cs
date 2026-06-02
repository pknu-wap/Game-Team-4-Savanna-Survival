using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
<<<<<<< Updated upstream
/// Skill_FantasyPassive 스킬 노드를 현재 열린 씬의 SkillContainer에 자동으로 추가한다.
/// 메뉴: Tools > SkillTree > Add FantasyPassive Node
=======
/// Fantasy 패시브 스킬 노드들을 현재 열린 씬의 SkillContainer(Fantasy 페이지)에 일괄 등록한다.
/// 메뉴: Tools > SkillTree > Register Fantasy Passive Nodes
>>>>>>> Stashed changes
/// </summary>
public static class FantasyPassiveNodeCreator
{
    private const string PrefabPath = "Assets/Prefabs/SkillNode.prefab";
<<<<<<< Updated upstream
    private const string SkillPath  = "Assets/ScriptableObjects/Skills/Fantasy/Skill_FantasyPassive.asset";

    [MenuItem("Tools/SkillTree/Add FantasyPassive Node")]
    public static void AddNode()
    {
        // 1) SkillNode 프리팹 로드
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[FantasyPassiveNode] SkillNode 프리팹을 찾을 수 없습니다: {PrefabPath}");
            return;
        }

        // 2) 스킬 데이터 로드
        BaseSkillData skill = AssetDatabase.LoadAssetAtPath<BaseSkillData>(SkillPath);
        if (skill == null)
        {
            Debug.LogError($"[FantasyPassiveNode] Skill_FantasyPassive 에셋을 찾을 수 없습니다: {SkillPath}");
            return;
        }

        // 3) 씬에서 SkillTreeWindow(비활성 포함) 찾기
        SkillTreeWindow window = Object.FindObjectOfType<SkillTreeWindow>(true);
        if (window == null)
        {
            Debug.LogError("[FantasyPassiveNode] 현재 씬에 SkillTreeWindow가 없습니다. 스킬 트리가 있는 씬을 열어주세요.");
            return;
        }

        // 4) SkillContainer 찾기
        Transform container = window.transform.Find("SkillContainer");
        if (container == null)
        {
            Debug.LogError("[FantasyPassiveNode] SkillTreeWindow 아래에 SkillContainer를 찾을 수 없습니다.");
            return;
        }

        // 5) 노드를 넣을 페이지 선택 (Fantasy 트리 = 마지막 페이지로 가정, 없으면 컨테이너 직속)
=======

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
>>>>>>> Stashed changes
        Transform page = container.childCount > 0
            ? container.GetChild(container.childCount - 1)
            : container;

<<<<<<< Updated upstream
        // 6) 중복 추가 방지
        foreach (var existing in page.GetComponentsInChildren<SkillTreeNodeUI>(true))
        {
            if (existing.SkillData == skill)
            {
                Debug.LogWarning("[FantasyPassiveNode] 이미 해당 스킬 노드가 존재합니다. 추가를 건너뜁니다.");
                Selection.activeGameObject = existing.gameObject;
                return;
            }
        }

        // 7) 프리팹 인스턴스 생성 (프리팹 연결 유지)
        GameObject node = (GameObject)PrefabUtility.InstantiatePrefab(prefab, page);
        node.name = skill.skillName;

        // 8) skillData(private 직렬화 필드) 할당
        SkillTreeNodeUI nodeUI = node.GetComponent<SkillTreeNodeUI>();
        SerializedObject so = new SerializedObject(nodeUI);
        SerializedProperty prop = so.FindProperty("skillData");
        if (prop != null)
        {
            prop.objectReferenceValue = skill;
            so.ApplyModifiedProperties();
        }
        else
        {
            Debug.LogWarning("[FantasyPassiveNode] skillData 필드를 찾지 못했습니다. 수동 할당이 필요합니다.");
        }

        // 9) 위치 설정 (treePosition 기준)
        RectTransform rt = node.GetComponent<RectTransform>();
        if (rt != null)
            rt.anchoredPosition = skill.treePosition;

        // 10) 변경사항 저장 표시 + 선택
        Undo.RegisterCreatedObjectUndo(node, "Add FantasyPassive Node");
        EditorSceneManager.MarkSceneDirty(node.scene);
        Selection.activeGameObject = node;

        Debug.Log($"[FantasyPassiveNode] '{skill.skillName}' 노드를 '{page.name}'에 추가했습니다. (위치 {skill.treePosition})");
=======
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
>>>>>>> Stashed changes
    }
}
