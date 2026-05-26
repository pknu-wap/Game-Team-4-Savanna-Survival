using UnityEngine;
using UnityEngine.EventSystems;

public class SucceedUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI")]
    // [SerializeField] private TMP_Text equipmentNameText;
    [SerializeField] private HighlightSelectedUI highlightSelectedUI;

    private SucceedUIManager succeedUIManager;
    private EquipmentData equipment;

    public EquipmentData Equipment => equipment;

    public void initialize(SucceedUIManager manager, EquipmentData equipmentData)
    {
        succeedUIManager = manager;
        equipment = equipmentData;

        // 장비 이름은 슬롯에 표시하지 않고, hover 시 ItemInfoUI에서 확인한다.
        // if (equipmentNameText != null)
        // {
        //     equipmentNameText.text = equipment != null ? equipment.equipmentName : "";
        // }

        setSelected(false);
    }

    public void setSelected(bool isSelected)
    {
        if (highlightSelectedUI != null)
        {
            highlightSelectedUI.setHighlighted(isSelected);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        succeedUIManager?.onSlotHover(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        succeedUIManager?.onSlotExit(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        succeedUIManager?.onSlotClick(this);
    }
}
