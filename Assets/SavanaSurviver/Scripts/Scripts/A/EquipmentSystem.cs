using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    private PlayerStat playerStat;

    private void Awake()
    {
        playerStat = GetComponent<PlayerStat>();                //PlayerStat을 GetComponent로 받아와서, 플레이어의 인스펙터에 붙여야함
    }

    public void applyModifier(statChange statChange)
    {//스탯을 변경하는 함수
        switch (statChange.statType)
        {//statType에 따라 분류
            case statType.maxHp:                                //스탯=maxHp면
                playerStat.playerMaxHp = calculateValue(        //PlayerStat 스크립트의 playerMaxHp값 계산
                    playerStat.playerMaxHp,                     //현재체력
                    statChange.statChangeType,                  //변환타입 (더하기, 곱하기)
                    statChange.value                            //값
                );
                Debug.Log("체력 " + playerStat.playerMaxHp + " " + statChange.statChangeType + " " + statChange.value);
                break;
                                                                //이하동일
            case statType.attack:
                playerStat.playerAttack = calculateValue(
                    playerStat.playerAttack,
                    statChange.statChangeType,
                    statChange.value
                );
                Debug.Log("공격력 " + playerStat.playerAttack + " " + statChange.statChangeType + " " + statChange.value);
                break;

            case statType.moveSpeed:
                playerStat.playerSpeed = calculateValue(
                    playerStat.playerSpeed,
                    statChange.statChangeType,
                    statChange.value
                );
                Debug.Log("이동속도 " + playerStat.playerSpeed + " " + statChange.statChangeType + " " + statChange.value);
                break;

            case statType.skillCooldown:
                playerStat.playerSkillcooldown = calculateValue(
                    playerStat.playerSkillcooldown,
                    statChange.statChangeType,
                    statChange.value
                );
                Debug.Log("쿨감 " + playerStat.playerSkillcooldown + " " + statChange.statChangeType + " " + statChange.value);
                break;

            case statType.skillDamage:
                playerStat.playerSkillDamage = calculateValue(
                    playerStat.playerSkillDamage,
                    statChange.statChangeType,
                    statChange.value
                );
                Debug.Log("스킬뎀 " + playerStat.playerSkillDamage + " " + statChange.statChangeType + " " + statChange.value);
                break;
        }
    }

    private float calculateValue(float currentValue, statChangeType statChangeType, float amount) //현재 값, 연산타입, 증가량
    {//스탯을 연산 방식에 따라 보정하는 함수
        switch (statChangeType)
        {
            case statChangeType.Add: //연산타입 더하기
                return currentValue + amount;

            case statChangeType.Multiply: //연산타입 곱하기
                return currentValue * amount;

            default: //정해지지 않았다면 그대로반환
                return currentValue;
        }
    }
}