using UnityEngine;
using UnityEngine.SceneManagement;

public class BossMapManager : MonoBehaviour
{
    [Header("보스 등장 시간 설정")]
    [SerializeField] private float bossAppearTime = 10f;

    [Header("보스맵 이동 UI")]
    [SerializeField] private GameObject bossMapMovePanel;

    [Header("UI 등장 후 정지까지 걸리는 시간")]
    [SerializeField] private float stopDelay = 10f;

    [Header("보스 씬 이름")]
    [SerializeField] private string bossSceneName = "Boss Scene";

    private float currentTime;
    private bool bossUiOpened;
    private bool timeStopped;

    private void Start()
    {
        if (bossMapMovePanel != null)
            bossMapMovePanel.SetActive(false);
    }

    private void Update()
    {
        if (bossUiOpened) return;

        currentTime += Time.deltaTime;

        if (currentTime >= bossAppearTime)
            OpenBossMapUI();
    }

    private void OpenBossMapUI()
    {
        bossUiOpened = true;

        if (bossMapMovePanel != null)
            bossMapMovePanel.SetActive(true);

        Invoke(nameof(StopGameTime), stopDelay);
    }

    private void StopGameTime()
    {
        if (timeStopped) return;

        timeStopped = true;
        Time.timeScale = 0f;

        Debug.Log("보스맵 UI 등장 후 10초 경과 : 게임 정지");
    }

    public void OnBossMapButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(bossSceneName);
    }
}