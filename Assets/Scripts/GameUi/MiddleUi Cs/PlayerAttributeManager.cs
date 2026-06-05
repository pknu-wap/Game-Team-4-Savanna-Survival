using UnityEngine;

public class PlayerAttributeManager : MonoBehaviour
{
    private static PlayerAttributeManager instance;

    [SerializeField] private PlayerStatManager playerStatManager;

    [Header("테스트 시작 포인트")]
    [SerializeField] private float startPoints = 30f;

    private PlayerAttribute attribute;
    private bool initialized = false;

    public static PlayerAttributeManager Instance => instance;

    public PlayerAttribute Attribute => attribute;


    private void Start()
    {
        Initialize();
    }

    public bool Initialize()
    {
        if (initialized)
            return attribute != null;

        if (playerStatManager == null)
            playerStatManager = FindFirstObjectByType<PlayerStatManager>();

        if (playerStatManager == null)
        {
            Debug.LogError("PlayerAttributeManager: PlayerStatManager를 찾지 못했습니다.");
            return false;
        }

        if (playerStatManager.StatCore == null)
        {
            Debug.LogError("PlayerAttributeManager: PlayerStatManager.StatCore가 null입니다.");
            return false;
        }

        attribute = new PlayerAttribute(playerStatManager.StatCore);

        attribute.grantPoints(startPoints);

        initialized = true;

        Debug.Log($"PlayerAttributeManager 초기화 완료 / 시작 포인트: {startPoints}");

        return true;
    }

    public class PlayerAttribute : AttributeManager
    {
        public PlayerAttribute(StatManager statManager) : base(statManager)
        {
        }
    }
}