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

        AugmentPowerDTO power = data.Power.Find(p => p.Level == level);
        if (power != null)
        {
            descriptionText.text = $"{data.Description}\n효과: +{power.Amount}";
        }
        else
        {
            descriptionText.text = data.Description;
        }
        

        // button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Debug.Log("asd");
        onSelected?.Invoke(augmentData);
    }
}
