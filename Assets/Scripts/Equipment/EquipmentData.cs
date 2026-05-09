using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentData", menuName = "Equipment/Equipment Data")]
public class EquipmentData : ScriptableObject
{
    [Header("장비 정보")]
    public int inInventoryId = 0;
    public bool isEquip = false;
    public string equipmentId;
    public string equipmentName;

    [Header("장비 스탯")]
    public List<EquipmentStat> equipmentStats = new List<EquipmentStat>();
}
