using UnityEngine;
using System;

public class EquipmentInventory : MonoBehaviour
{
    [Header("보유한 장비")]
    private EquipmentInventoryData[] bagItems = new EquipmentInventoryData[15];
    private EquipmentInventoryData[] equippedItems = new EquipmentInventoryData[5];

    // private int nextInventoryId = 0;

    public event Action onInventoryChanged;

    [SerializeField] private PlayerStatManager playerStatManager;

    public EquipmentData getBag(int index)
    {
        if (bagItems[index] == null)
        {
            Debug.LogWarning("null return");
            return null;
        }
        return bagItems[index].equipmentData;
    }

    public EquipmentData getEquipped(int index)
    {
        if (equippedItems[index] == null)
        {
            Debug.LogWarning("null return");
            return null;
        }
        return equippedItems[index].equipmentData;
    }

    public void setBag(int index, EquipmentData equipment)
    {
        if (equipment == null)
        {
            bagItems[index] = null;
        }
        bagItems[index] = new EquipmentInventoryData(equipment);
        notifyInventoryChanged();
    }

    public void setEquipped(int index, EquipmentData equipment)
    {
        if (equipment == null)
        {
            equippedItems[index] = null;
        }
        equippedItems[index] = new EquipmentInventoryData(equipment);
        notifyInventoryChanged();
    }

    // 장비를 얻었을 때 빈 인벤토리 위치에 획득
    public bool addInventoryEquipment(EquipmentData equipment)
    {
        EquipmentInventoryData item = new EquipmentInventoryData(equipment);

        int emptySlotIndex = getEmptyEquippedSlotIndex(equippedItems);

        if (emptySlotIndex >= 0)
        {
           equippedItems[emptySlotIndex] = item;
           addEquipmentStat(item.equipmentData);

           Debug.Log("장비장착: " + equipment.equipmentName);

           notifyInventoryChanged();
           return true;
        }

        emptySlotIndex = getEmptyEquippedSlotIndex(bagItems);
        if (emptySlotIndex >= 0)
        {
            bagItems[emptySlotIndex] = item;

            Debug.Log("장비보관: " + equipment.equipmentName);

            notifyInventoryChanged();
            return true;
        }

        Debug.Log("획득실패");
        notifyInventoryChanged();
        return false;
    }

    // 장비 버리는 함수
    public bool removeInventoryEquipment(EquipmentData equipment)
    {
        // 미구현
        return false;
    }

    /*
    private bool isValidIndex(EquipmentInventoryData[] inventory, int index)
    {
        if (index >= 0 && index < inventory.Length) return true;
        else
        {
            Debug.LogError("inventory index 오류");
            return false;
        }
    }
    */

    public void addEquipmentStat(EquipmentData equipment)
    {
        playerStatManager.applyEquipmentStat(equipment.equipmentStats);
    }

    public void removeEquipmentStat(EquipmentData equipment)
    {
        playerStatManager.removeEquipmentStat(equipment.equipmentStats);
    }

    private int getEmptyEquippedSlotIndex(EquipmentInventoryData[] inventory) 
    {
        for(int i = 0; i < inventory.Length; ++i)
        {
            if (inventory[i] == null) return i;
        }
        return -1;
    }

    // 이벤트 둘러보니까 미사용임. 이벤트 형식으로 변경할 것
    private void notifyInventoryChanged()
    {
        onInventoryChanged?.Invoke();
    }
}
