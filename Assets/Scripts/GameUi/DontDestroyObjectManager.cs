using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoadObject : MonoBehaviour
{
    private static DontDestroyOnLoadObject instance;

    [Header("씬 이동 시 현재 스탯 초기화")]
    [SerializeField] private bool resetCurrentStatsOnSceneLoaded = true;

    [Header("초기화할 현재 스탯")]
    [SerializeField] private bool resetHealth = true;
    [SerializeField] private bool resetHunger = true;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!resetCurrentStatsOnSceneLoaded)
            return;

        ResetCurrentStats();
    }

    public void ResetCurrentStats()
    {
        PlayerStatManager playerStatManager = FindObjectOfType<PlayerStatManager>();

        if (playerStatManager == null)
        {
            Debug.LogWarning("PlayerStatManager를 찾을 수 없어 현재 스탯 초기화를 건너뜁니다.");
            return;
        }

        PlayerStatCore statCore = playerStatManager.StatCore;

        if (statCore == null)
        {
            Debug.LogWarning("PlayerStatCore가 없어 현재 스탯 초기화를 건너뜁니다.");
            return;
        }

        if (resetHealth)
        {
            float maxHealth = statCore.getStat(StatType.MAX_HEALTH).calibratedValue;
            statCore.registerStat(StatType.HEALTH, maxHealth);
        }

        if (resetHunger)
        {
            float maxHunger = statCore.getStat(StatType.MAX_HUNGER).calibratedValue;
            statCore.registerStat(StatType.HUNGER, maxHunger);
        }

        Debug.Log("씬 이동 후 현재 체력/배고픔 초기화 완료");
    }
}