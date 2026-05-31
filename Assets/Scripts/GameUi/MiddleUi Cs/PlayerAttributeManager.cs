using UnityEngine;

public class PlayerAttributeManager : MonoBehaviour
{
    [SerializeField] private PlayerStatManager playerStatManager;

    [Header("테스트 시작 포인트")]
    [SerializeField] private float startPoints = 30f;

    private PlayerAttribute attribute;

    public PlayerAttribute Attribute => attribute;

    private void Awake()
    {
        if (playerStatManager == null)
            playerStatManager = FindObjectOfType<PlayerStatManager>();

        attribute = new PlayerAttribute(playerStatManager.StatCore);

        // 테스트용 시작 포인트 지급
        attribute.grantPoints(startPoints);
    }

    public class PlayerAttribute : AttributeManager
    {
        public PlayerAttribute(StatManager statManager) : base(statManager)
        {
        }
    }
}