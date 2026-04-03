using System.Collections.Generic;
using UnityEngine;

public class EquipmentTemp : MonoBehaviour
{                                                       //장비에 붙이면됨
    [Header("장비 스탯")]
    public List<statChange> Equipments = new List<statChange>();
    //인스펙터에서 장비가 가지는 스탯 입력

    private void OnTriggerEnter2D(Collider2D other)     //충돌시
    {                                                   //충돌을 플레이어와 하기에, 플레이어의 EquipmentSystem Script 불러오기
        EquipmentSystem equipmentSystem = other.GetComponent <EquipmentSystem>();
        foreach (statChange stat in Equipments)         
        {
            equipmentSystem.applyModifier(stat);        //스탯꺼내와서 스탯변경함수실행
        }
        Destroy(gameObject);                            //장비안보이게
    }
}