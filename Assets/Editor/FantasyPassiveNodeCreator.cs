using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Skill_FantasyPassive 스킬 노드를 현재 열린 씬의 SkillContainer에 자동으로 추가한다.
/// 메뉴: Tools > SkillTree > Add FantasyPassive Node
/// </summary>
public static class FantasyPassiveNodeCreator
{
    private const string PrefabPath = "Assets/Prefabs/SkillNode.prefab";
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
        Transform page = container.childCount > 0
            ? container.GetChild(container.childCount - 1)
            : container;

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
    }
}
