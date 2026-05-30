using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SucceedEquipmentDatabase", menuName = "Equipment/Succeed Equipment Database")]
public class SucceedEquipmentDatabase : ScriptableObject
{
    [SerializeField] private List<EquipmentData> equipments = new List<EquipmentData>();

    public EquipmentData getEquipment(string equipmentId)
    {
        for (int i = 0; i < equipments.Count; ++i)
        {
            if (equipments[i] != null && equipments[i].equipmentId == equipmentId)
            {
                return equipments[i];
            }
        }

        return null;
    }
}
