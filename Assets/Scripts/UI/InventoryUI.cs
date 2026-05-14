using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private EquipmentInventory equipmentInventory;
    [SerializeField] private GameObject inventoryPanel;
    
    [SerializeField] private InventoryDrag[] equippedSlots;
    [SerializeField] private InventoryDrag[] bagSlots;
    
    private void refreshUI()
    {
        for (int i = 0; i < equippedSlots.Length; ++i)
        {
            equippedSlots[i].setSlot(equipmentInventory.getEquipped(i));
        }

        for (int i = 0; i < bagSlots.Length; ++i)
        {
            bagSlots[i].setSlot(equipmentInventory.getBag(i));
        }
    }

    private void Start()
    {
        equipmentInventory.onInventoryChanged += refreshUI;
        refreshUI();
    }

    /*
    public void openInventoryUI()
    {
        inventoryPanel.SetActive(true);
    }
    */
}
