using UnityEngine;

public class StrengthUiManager : MonoBehaviour
{
    [SerializeField] private PlayerAttributeManager playerAttributeManager;

    [Header("강화할 스탯")]
    [SerializeField] private StatType statType;

    [Header("버튼 1회당 소모 포인트")]
    [SerializeField] private float investPoint = 1f;

    private AttributeManager attribute;

    private void Start()
    {
        if (playerAttributeManager == null)
            playerAttributeManager = FindObjectOfType<PlayerAttributeManager>();

        attribute = playerAttributeManager.Attribute;
    }

    public void OnClickStrengthButton()
    {
        if (attribute == null)
        {
            Debug.LogError("AttributeManager가 연결되지 않았습니다.");
            return;
        }

        bool success = attribute.investPoint(statType, investPoint);

        if (!success)
        {
            Debug.Log("스탯 포인트가 부족합니다.");
            return;
        }

        Debug.Log($"{statType} 강화 완료");
    }
}