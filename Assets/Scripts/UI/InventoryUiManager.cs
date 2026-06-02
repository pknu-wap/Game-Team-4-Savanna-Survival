using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum EquipmentSlotArea
{
    Inventory,
    Equipped
}

public class InventoryUiManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("슬롯")]
    // 인벤 위아래 구분
    [SerializeField] private EquipmentSlotArea slotArea;
    // 각 슬롯 인덱스
    [SerializeField] private int slotIndex;

    [Header("UI 표시")]
    // 이 슬롯의 장비 텍스트
    // [SerializeField] private TMP_Text slotNameText;
    // image

    [Header("스크립트")]
    [SerializeField] private EquipmentInventory equipmentInventory;
    [SerializeField] private InventoryMove inventoryMove;
    [SerializeField] private ItemInfoUI ItemInfoUI;

    // 현재 드래그중인 장비
    private EquipmentData currentEquipment;
    private static InventoryUiManager currentDragging;

    private void Awake()
    {
        if (ItemInfoUI == null)
        {
            ItemInfoUI = FindObjectOfType<ItemInfoUI>(true);
        }
    }

    public void setSlot(EquipmentData equipment)
    {
        currentEquipment = equipment;
        if (currentEquipment == null)
        {
            // slotNameText.text = "";
            // image false
            return;
        }
        // slotNameText.text = currentEquipment.equipmentName;
        // image true
        // 분리해야할듯
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentEquipment == null) return;
        if (ItemInfoUI == null) return;
        ItemInfoUI.openEquipmentInfo(currentEquipment);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemInfoUI == null) return;
        ItemInfoUI.hideEquipmentInfo();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 빈 슬롯
        if (currentEquipment == null)
        {
            currentDragging = null;
            return;
        }
        currentDragging = this;
        // Debug.Log("드래그 시작");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("드래그 중");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log("드래그 끝");
    }

    // 다른 슬롯 위에 드롭했을 때 호출됨
    public void OnDrop(PointerEventData eventData)
    {
        // Debug.Log("드래그 드랍 호출");
        // 드래그 시작 슬롯이 없으면
        if (currentDragging == null)
        {
            return;
        }

        // 자기 자신에게 드롭한 경우
        if (currentDragging == this)
        {
            currentDragging = null;
            return;
        }

        // 드래그한 슬롯의 장비
        EquipmentData tempFromEquipment = currentDragging.currentEquipment;

        // 드롭한 슬롯의 기존 장비
        EquipmentData tempToEquipment = currentEquipment;

        // 인벤 -> 장착
        if (currentDragging.slotArea == EquipmentSlotArea.Inventory && slotArea == EquipmentSlotArea.Equipped)
        {
            inventoryMove.moveBagToEquip(
                currentDragging.slotIndex,
                slotIndex,
                tempFromEquipment,
                tempToEquipment
            );
            Debug.Log("인벤 -> 장착");
        }
        // 인벤 -> 인벤
        else if (currentDragging.slotArea == EquipmentSlotArea.Inventory && slotArea == EquipmentSlotArea.Inventory)
        {
            inventoryMove.moveBagToBag(
                currentDragging.slotIndex,
                slotIndex,
                tempFromEquipment,
                tempToEquipment
            );
            Debug.Log("인벤 -> 인벤");
        }
        // 장착 → 인벤
        else if (currentDragging.slotArea == EquipmentSlotArea.Equipped && slotArea == EquipmentSlotArea.Inventory)
        {
            inventoryMove.moveEquipToBag(
                currentDragging.slotIndex,
                slotIndex,
                tempFromEquipment,
                tempToEquipment
            );
            Debug.Log("장착 -> 인벤");
        }
        // 장착 → 장착
        else if (currentDragging.slotArea == EquipmentSlotArea.Equipped && slotArea == EquipmentSlotArea.Equipped)
        {
            inventoryMove.moveEquipToEquip(
                currentDragging.slotIndex,
                slotIndex,
                tempFromEquipment,
                tempToEquipment
            );
            Debug.Log("장착 -> 장착");
        }
        currentDragging = null;
        // Debug.Log("드래그 완료");
    }
}
