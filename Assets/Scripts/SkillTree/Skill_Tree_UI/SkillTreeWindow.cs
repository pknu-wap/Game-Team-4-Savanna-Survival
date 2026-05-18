using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillTreeWindow : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform lineContainer;
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button rightArrowButton;
    [SerializeField] private Color lockedLineColor = Color.gray;
    [SerializeField] private Color unlockedLineColor = new Color(0.2f, 0.6f, 1f);

    private List<Transform> pageContainers = new();
    private int currentPage = 0;
    private bool isWindowOpen = false;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        // Find all page containers (direct children of NodesContainer)
        Transform nodesContainer = transform.Find("SkillContainer");
        if (nodesContainer != null)
        {
            foreach (Transform child in nodesContainer)
            {
                pageContainers.Add(child);
            }
        }

        // Subscribe to arrow buttons
        if (leftArrowButton != null)
            leftArrowButton.onClick.AddListener(PreviousPage);
        if (rightArrowButton != null)
            rightArrowButton.onClick.AddListener(NextPage);

        // Subscribe to skill unlock events
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnSkillUnlocked += OnSkillUnlocked;
        }

        // Initialize window state
        SetWindowOpen(false);
        UpdatePageDisplay();
    }

    private void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePageDisplay();
        }
    }

    private void NextPage()
    {
        if (currentPage < pageContainers.Count - 1)
        {
            currentPage++;
            UpdatePageDisplay();
        }
    }

    private void UpdatePageDisplay()
    {
        for (int i = 0; i < pageContainers.Count; i++)
        {
            pageContainers[i].gameObject.SetActive(i == currentPage);
        }

        // Update arrow button states
        if (leftArrowButton != null)
            leftArrowButton.interactable = (currentPage > 0);
        if (rightArrowButton != null)
            rightArrowButton.interactable = (currentPage < pageContainers.Count - 1);

        DrawConnectionLines();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SetWindowOpen(!isWindowOpen);
        }
    }

    private void SetWindowOpen(bool open)
    {
        isWindowOpen = open;
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = open ? 1f : 0f;
            canvasGroup.blocksRaycasts = open;
            canvasGroup.interactable = open;
        }

        if (canvas != null)
        {
            canvas.enabled = open;
        }

        if (open)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        DrawConnectionLines();
    }

    private void DrawConnectionLines()
    {
        if (lineContainer == null) return;

        // Clear existing lines
        foreach (Transform child in lineContainer)
        {
            Destroy(child.gameObject);
        }

        // Get nodes from current page only
        if (currentPage >= pageContainers.Count) return;

        List<SkillTreeNodeUI> currentPageNodes = new();
        currentPageNodes.AddRange(pageContainers[currentPage].GetComponentsInChildren<SkillTreeNodeUI>());

        // Draw lines between connected skills
        foreach (var node in currentPageNodes)
        {
            if (node.SkillData == null || node.SkillData.prerequisites.Count == 0)
                continue;

            foreach (var prerequisite in node.SkillData.prerequisites)
            {
                var prerequisiteNode = currentPageNodes.Find(n => n.SkillData == prerequisite);
                if (prerequisiteNode == null) continue;

                bool isUnlocked = SkillManager.Instance.IsUnlocked(prerequisite);
                DrawLine(prerequisiteNode.GetComponent<RectTransform>(),
                         node.GetComponent<RectTransform>(),
                         isUnlocked ? unlockedLineColor : lockedLineColor);
            }
        }
    }

    private void DrawLine(RectTransform from, RectTransform to, Color color)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(lineContainer, false);

        Image lineImage = lineObj.AddComponent<Image>();
        lineImage.color = color;

        RectTransform lineRect = lineObj.GetComponent<RectTransform>();
        lineRect.anchorMin = new Vector2(0.5f, 0.5f);
        lineRect.anchorMax = new Vector2(0.5f, 0.5f);
        lineRect.pivot = new Vector2(0.5f, 0.5f);

        // 각 노드의 월드 좌표를 lineContainer 로컬 좌표로 변환
        Vector2 fromPos = lineContainer.InverseTransformPoint(from.position);
        Vector2 toPos = lineContainer.InverseTransformPoint(to.position);
        Vector2 direction = (toPos - fromPos).normalized;
        float distance = Vector2.Distance(fromPos, toPos);

        lineRect.sizeDelta = new Vector2(distance, 2f);
        lineRect.anchoredPosition = (fromPos + toPos) * 0.5f;
        lineRect.localRotation = Quaternion.FromToRotation(Vector3.right, new Vector3(direction.x, direction.y, 0f));
    }

    private void OnSkillUnlocked(BaseSkillData skill)
    {
        DrawConnectionLines();
    }

    private void OnDestroy()
    {
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnSkillUnlocked -= OnSkillUnlocked;
        }

        Time.timeScale = 1f;
    }
}
