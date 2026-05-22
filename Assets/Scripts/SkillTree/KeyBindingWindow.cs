using UnityEngine;
using UnityEngine.UI;

public class KeyBindingWindow : MonoBehaviour
{
    public static KeyBindingWindow Instance { get; private set; }

    [SerializeField] private Text skillNameText;
    [SerializeField] private Text currentKeyText;
    [SerializeField] private Text promptText;
    [SerializeField] private CanvasGroup canvasGroup;

    private static readonly KeyCode[] allKeyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));

    private ActiveSkillData targetSkill;
    private PlayerSkillController controller;
    private bool isListening = false;

    private void Awake()
    {
        Instance = this;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        isListening = false;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }

    private void Update()
    {
        if (!isListening) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
            return;
        }

        foreach (KeyCode kc in allKeyCodes)
        {
            if (kc == KeyCode.Escape) continue;
            if (kc == KeyCode.Mouse0 || kc == KeyCode.Mouse1 || kc == KeyCode.Mouse2) continue;
            if (kc == KeyCode.None) continue;

            if (Input.GetKeyDown(kc))
            {
                controller.SetBinding(targetSkill, kc);
                Close();
                return;
            }
        }
    }

    public void Open(ActiveSkillData skill, PlayerSkillController ctrl)
    {
        targetSkill = skill;
        controller = ctrl;

        if (skillNameText != null)
            skillNameText.text = skill.skillName;

        if (currentKeyText != null)
        {
            KeyCode current = ctrl.GetBinding(skill);
            currentKeyText.text = current == KeyCode.None ? "현재 키: 없음" : $"현재 키: {current}";
        }

        if (promptText != null)
            promptText.text = "사용할 키를 누르세요\n(ESC: 취소)";

        gameObject.SetActive(true); // Awake가 여기서 실행될 수 있으므로 isListening은 반드시 이후에 설정

        isListening = true;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
    }

    private void Close()
    {
        isListening = false;
        gameObject.SetActive(false);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }
}
