using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    private PlayerStat playerStat;

    private void Awake()
    {
        playerStat = GetComponent<PlayerStat>();
    }

    public void applyModifier(statChange statChange)
    {
        switch (statChange.statType)
        {
            case statType.maxHp:
                playerStat.playerMaxHp = calculateValue(
                    playerStat.playerMaxHp,
                    statChange.statChangeType,
                    statChange.value
                );
                break;

            case statType.attack:
                playerStat.playerAttack = calculateValue(
                    playerStat.playerAttack,
                    statChange.statChangeType,
                    statChange.value
                );
                break;

            case statType.moveSpeed:
                playerStat.playerSpeed = calculateValue(
                    playerStat.playerSpeed,
                    statChange.statChangeType,
                    statChange.value
                );
                break;

            case statType.skillCooldown:
                playerStat.playerSkillcooldown = calculateValue(
                    playerStat.playerSkillcooldown,
                    statChange.statChangeType,
                    statChange.value
                );
                break;

            case statType.skillDamage:
                playerStat.playerSkillDamage = calculateValue(
                    playerStat.playerSkillDamage,
                    statChange.statChangeType,
                    statChange.value
                );
                break;
        }
    }

    private float calculateValue(float currentValue, statChangeType statChangeType, float amount)
    {
        switch (statChangeType)
        {
            case statChangeType.Add:
                return currentValue + amount;

            case statChangeType.Multiply:
                return currentValue * amount;

            default:
                return currentValue;
        }
    }
}