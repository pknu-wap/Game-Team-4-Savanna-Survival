using System.Collections.Generic;
using UnityEngine;

public class EquipmentTemp : MonoBehaviour
{
    [Header("이 장비가 줄 스탯 변화값")]
    public List<statChange> Equipments = new List<statChange>();
    
    private void OnTriggerEnter2D(Collider2D other) //충돌시 장비되게
    {
        EquipmentSystem equipmentSystem = other.GetComponent<EquipmentSystem>();
        foreach (statChange stat in Equipments)
        {
            equipmentSystem.applyModifier(stat);
        }
        Destroy(gameObject);
    }
}