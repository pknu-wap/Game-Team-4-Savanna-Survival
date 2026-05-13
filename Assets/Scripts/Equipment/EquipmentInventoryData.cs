using System;

[Serializable]
public class EquipmentInventoryData
{
    // public int inventoryId;
    public EquipmentData equipmentData;

    public EquipmentInventoryData(/*int inventoryId, */EquipmentData equipmentData)
    {
        // this.inventoryId = inventoryId;
        this.equipmentData = equipmentData;
    }
}