using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum EquipmentSlotArea
{
    Inventory,
    Equipped
}

public class InventoryDrag : MonoBehaviour, IBeginDragHandler, IDropHandler
{
    [Header("슬롯")]
    // 인벤 위아래 구분
    [SerializeField] private EquipmentSlotArea slotArea;
    // 각 슬롯 인덱스
    [SerializeField] private int slotIndex;

    [Header("UI 표시")]
    // 이 슬롯의 장비 텍스트
    [SerializeField] private TMP_Text slotNameText;

    [Header("스크립트")]
    [SerializeField] private EquipmentInventory equipmentInventory;
    [SerializeField] private InventoryMove inventoryMove;

    // 현재 드래그중인 장비
    private EquipmentData currentEquipment;
    private static InventoryDrag currentDragging;
    
    // get
    public EquipmentSlotArea SlotArea
    {
        get { return slotArea; }
    }
    public int SlotIndex
    {
        get { return slotIndex; }
    }
    public EquipmentData CurrentEquipment
    {
        get { return currentEquipment; }
    }

    public void setSlot(EquipmentData equipment)
    {
        currentEquipment = equipment;
        if (currentEquipment = null)
        {
            slotNameText.text = "";
            // image false
            return;
        }
        slotNameText.text = currentEquipment.equipmentName;
        // image true
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
    }

    // 다른 슬롯 위에 드롭했을 때 호출됨
    public void OnDrop(PointerEventData eventData)
    {
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
        EquipmentData tempFromEquipment = currentDragging.CurrentEquipment;

        // 드롭한 슬롯의 기존 장비
        EquipmentData tempToEquipment = currentEquipment;

        // 인벤 -> 장착
        if (currentDragging.SlotArea == EquipmentSlotArea.Inventory && slotArea == EquipmentSlotArea.Equipped)
        {
            inventoryMove.moveBagToEquip(
                currentDragging.SlotIndex,
                slotIndex,
                tempFromEquipment,
                tempToEquipment
            );
        }
        // 인벤 -> 인벤
        else if (currentDragging.SlotArea == EquipmentSlotArea.Inventory && slotArea == EquipmentSlotArea.Inventory)
        {
            inventoryMove.moveBagToBag(
                currentDragging.SlotIndex,
                slotIndex,
                tempFromEquipment,
                tempToEquipment
            );
        }
        // 장착 → 인벤
        else if (currentDragging.SlotArea == EquipmentSlotArea.Equipped && slotArea == EquipmentSlotArea.Inventory)
        {
            inventoryMove.moveEquipToBag(
                currentDragging.SlotIndex,
                slotIndex,
                tempFromEquipment,
                tempToEquipment
            );
        }
        // 장착 → 장착
        else if (currentDragging.SlotArea == EquipmentSlotArea.Equipped && slotArea == EquipmentSlotArea.Equipped)
        {
            inventoryMove.moveEquipToEquip(
                currentDragging.SlotIndex,
                slotIndex,
                tempFromEquipment,
                tempToEquipment
            );
        }
        currentDragging = null;
    }
}