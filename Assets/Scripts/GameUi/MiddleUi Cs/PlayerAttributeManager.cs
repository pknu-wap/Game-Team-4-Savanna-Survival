using UnityEngine;

public class PlayerAttributeManager : MonoBehaviour
{
    [SerializeField] private PlayerStatManager playerStatManager;

    [Header("테스트 시작 포인트")]
    [SerializeField] private float startPoints = 30f;

    private PlayerAttribute attribute;

    public PlayerAttribute Attribute
    {
        get
        {
            if (attribute == null)
                Initialize();

            return attribute;
        }
    }

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        Initialize();
    }

    private bool Initialize()
    {
        if (attribute != null)
            return true;

        if (playerStatManager == null)
            playerStatManager = FindObjectOfType<PlayerStatManager>();

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