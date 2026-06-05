using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AugmentChoiceButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button button;

    private AugmentDTO augmentData;
    private Action<AugmentDTO> onSelected;

    public void Setup(AugmentDTO data, int level, Action<AugmentDTO> callback)
    {
        augmentData = data;
        onSelected = callback;

        nameText.text = data.Name;

        descriptionText.text = data.Description;


        // button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        onSelected?.Invoke(augmentData);
        int id = augmentData.ID;

        // 실제 시스템 작동 코드 적용 필요. SO 데이터 파일 들어와야 함. 스킬 제작 후 적용.


        BaseSkillData originSkill = Resources.Load<BaseSkillData>("Skills/" + id);
        BaseSkillData newSkill = Resources.Load<BaseSkillData>("Skills/" + id);


        SkillManager.Instance.ReplaceSkill(originSkill, newSkill);
    }
}