using UnityEngine;

/// 게임 내 점수 누적 및 Attribute 포인트 변환.
/// 씬에 하나만 존재하는 싱글톤.
/// 게임 종료 시 FinalizeScore()를 호출하면 점수를 포인트로 변환해 PlayerAttributeManager에 지급합니다.
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Attribute 변환")]
    [Tooltip("이 점수마다 Attribute 포인트 1점 획득")]
    [SerializeField] private float scorePerAttributePoint = 200f;

    [SerializeField] private PlayerAttributeManager playerAttributeManager;

    public int CurrentScore { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (playerAttributeManager == null)
            playerAttributeManager = FindObjectOfType<PlayerAttributeManager>();
    }

    private void OnEnable()  => EnemyEvents.OnDeath += OnEnemyDeath;
    private void OnDisable() => EnemyEvents.OnDeath -= OnEnemyDeath;

    private void OnEnemyDeath(EnemyDeathEvent e)
    {
        if (e.getScore() <= 0) return;
        CurrentScore += e.getScore();
        Debug.Log($"[ScoreManager] +{e.getScore()} ({e.getVictim().name} 처치) / 누계: {CurrentScore}");
    }

    /// <summary>
    /// 게임 종료 시 호출.
    /// 점수를 Attribute 포인트로 변환해 PlayerAttributeManager에 지급합니다.
    /// </summary>
    public void FinalizeScore()
    {
        float earned = CurrentScore / scorePerAttributePoint;
        if (earned <= 0f)
        {
            Debug.Log("[ScoreManager] 획득 Attribute 포인트 없음");
            CurrentScore = 0;
            return;
        }

        if (playerAttributeManager == null)
            playerAttributeManager = FindObjectOfType<PlayerAttributeManager>();

        if (playerAttributeManager != null)
        {
            playerAttributeManager.Attribute.grantPoints(earned);
            Debug.Log($"[ScoreManager] 점수 {CurrentScore} → Attribute {earned}pt 지급");
        }
        else
        {
            Debug.LogWarning("[ScoreManager] PlayerAttributeManager를 찾지 못했습니다.");
        }

        CurrentScore = 0;
    }

    public void ResetScore() => CurrentScore = 0;
}