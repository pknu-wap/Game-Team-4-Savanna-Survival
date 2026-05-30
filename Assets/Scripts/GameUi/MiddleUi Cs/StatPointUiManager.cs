using TMPro;
using UnityEngine;

public class StatPointUiManager : MonoBehaviour
{
    [SerializeField] private PlayerAttributeManager playerAttributeManager;
    [SerializeField] private TMP_Text statPointText;

    private AttributeManager attribute;

    private void Start()
    {
        if (playerAttributeManager == null)
            playerAttributeManager = FindObjectOfType<PlayerAttributeManager>();

        if (playerAttributeManager == null)
        {
            Debug.LogError("PlayerAttributeManager를 찾지 못했습니다.");
            return;
        }

        if (statPointText == null)
        {
            Debug.LogError("Stat Point Text가 연결되지 않았습니다.");
            return;
        }

        attribute = playerAttributeManager.Attribute;

        if (attribute == null)
        {
            Debug.LogError("Attribute가 아직 생성되지 않았습니다.");
            return;
        }

        attribute.onPointsChanged += OnPointsChanged;
        RefreshUI();
    }

    private void OnDestroy()
    {
        if (attribute != null)
            attribute.onPointsChanged -= OnPointsChanged;
    }

    private void OnPointsChanged(float points)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        statPointText.text = $"Stat Point : {attribute.getPoints():0}";
    }
}