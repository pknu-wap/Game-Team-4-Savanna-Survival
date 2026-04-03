using System.Collections.Generic;
using UnityEngine;

public class EquipmentTemp2 : MonoBehaviour
{
    [Header("장비 스탯")]
    //인스펙터에서 장비가 가지는 스탯 입력
    public List<EquipmentStat> Equipments = new List<EquipmentStat>();
    

    private void OnTriggerEnter2D(Collider2D other) //충돌시
    {
        PlayerStatManager playerStatManager = other.GetComponent<PlayerStatManager>();

        playerStatManager.ApplyEquipmentList(Equipments); //장비스탯 적용
        Destroy(gameObject);
    }
}