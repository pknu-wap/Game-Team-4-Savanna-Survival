using UnityEngine;

public class PlayerStat : MonoBehaviour
                //플레이어 인스펙터에 붙여서 시각적으로 확인하는 스크립트
{
    [Header("현재 스탯")] //알아서 추후 추가가능, 타 스크립트 스탯 계산에 참조됨
    public float playerMaxHp = 100f;
    public float playerAttack = 10f;
    public float playerSpeed = 10f;
    public float playerSkillcooldown = 0f;
    public float playerSkillDamage = 0f;
}