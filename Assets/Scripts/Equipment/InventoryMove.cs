using UnityEngine;

public class InventoryMove : MonoBehaviour
{
    [SerializeField] private EquipmentInventory inventory;

    // 가방에서 장비
    public void moveBagToEquip(int bagIndex, int equipSlotIndex, EquipmentData fromEquipment, EquipmentData toEquipment)
    {
        if (fromEquipment == null)
        {
            return;
        }

        inventory.setEquipped(equipSlotIndex, fromEquipment);
        inventory.setBag(bagIndex, toEquipment);

        inventory.addEquipmentStat(fromEquipment);
        if (toEquipment != null)
        {
            inventory.removeEquipmentStat(toEquipment);
        }
    }

    // 가방에서 가방
    public void moveBagToBag(int fromIndex, int toIndex, EquipmentData fromEquipment, EquipmentData toEquipment)
    {
        if (fromEquipment == null)
        {
            return;
        }

        inventory.setBag(toIndex, fromEquipment);
        inventory.setBag(fromIndex, toEquipment);
    }

    // 장비에서 가방
    public void moveEquipToBag(int equipSlotIndex, int bagIndex, EquipmentData fromEquipment, EquipmentData toEquipment)
    {
        if (fromEquipment == null)
        {
            return;
        }

        inventory.setBag(bagIndex, fromEquipment);
        inventory.setEquipped(equipSlotIndex, toEquipment);

        inventory.removeEquipmentStat(fromEquipment);
        if (toEquipment != null)
        {
            inventory.addEquipmentStat(toEquipment);
        }
    }

    // 장비에서 장비
    public void moveEquipToEquip(int fromIndex, int toIndex, EquipmentData fromEquipment, EquipmentData toEquipment)
    {
        if (fromEquipment == null)
        {
            return;
        }

        inventory.setEquipped(toIndex, fromEquipment);
        inventory.setEquipped(fromIndex, toEquipment); 
    }
}
