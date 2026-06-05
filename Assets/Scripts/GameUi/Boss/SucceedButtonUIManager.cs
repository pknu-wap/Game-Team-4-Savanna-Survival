using UnityEngine;
using UnityEngine.UI;

public class SucceedButtonUIManager : MonoBehaviour
{
    [Header("계승 UI 관리자")]
    [SerializeField] private SucceedUIManager succeedUIManager;

    [Header("계승 버튼")]
    [SerializeField] private Button succeedOpenButton;

    private void Awake()
    {
        if (succeedOpenButton != null)
        {
            succeedOpenButton.onClick.RemoveListener(OnClickSucceedButton);
            succeedOpenButton.onClick.AddListener(OnClickSucceedButton);
        }
    }

    private void OnDestroy()
    {
        if (succeedOpenButton != null)
        {
            succeedOpenButton.onClick.RemoveListener(OnClickSucceedButton);
        }
    }

    public void OnClickSucceedButton()
    {
        if (succeedUIManager == null)
        {
            Debug.LogError("SucceedUIManager가 연결되지 않았습니다.");
            return;
        }

        succeedUIManager.triggerSucceedUI();
    }
}