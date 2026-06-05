using UnityEngine;

public class StatStrengthPanelManager : MonoBehaviour
{
    [Header("스탯 패널")]
    [SerializeField] private GameObject statPanel;

    private bool isOpen = false;

    private void Start()
    {
        isOpen = false;

        if (statPanel != null)
            statPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) ||
            Input.GetKeyDown(KeyCode.RightShift))
        {
            ToggleStatPanel();
        }
    }

    private void ToggleStatPanel()
    {
        if (statPanel == null)
        {
            Debug.LogError("Stat Panel이 연결되지 않았습니다.");
            return;
        }

        isOpen = !isOpen;

        statPanel.SetActive(isOpen);

        // 열리면 시간 정지, 닫히면 시간 재개
        Time.timeScale = isOpen ? 0f : 1f;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}