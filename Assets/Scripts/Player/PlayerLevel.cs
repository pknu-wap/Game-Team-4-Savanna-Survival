using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public PlayerStatCore statCore;
    private PlayerMovement playerMovement;
    private float currentExp;
    private float maxExp;
    private float level;

    [Header("레벨업 보상")]
    public int skillPointsPerLevel = 1;

    void Start()
    {
        PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
        statCore = playerStatManager.StatCore;

        playerMovement = GetComponent<PlayerMovement>();
    }

    public void addExp(float value)
    {
        currentExp = statCore.getStat(StatType.EXP).rawValue;
        maxExp = statCore.getStat(StatType.MAX_EXP).rawValue;
        level = statCore.getStat(StatType.LEVEL).rawValue;

        currentExp += value;

        while (currentExp >= maxExp)
        {
            playerMovement.stopMove();

            currentExp -= maxExp;
            level++;
            maxExp *= 1.2f; //임시 레벨업 경험치통 증가 수치

            statCore.addStat(StatType.SKILL_POINTS, skillPointsPerLevel);

            if (AugmentManager.Instance != null)
            {
                AugmentManager.Instance.OpenAugment((int)level);
            }
            else
            {
                Debug.Log("PlayerLevel: AugmentManager.Instance가 없음");
            }
            Debug.LogError("레벨업! 현재 레벨: " + level);
        }

        statCore.registerStat(StatType.EXP, currentExp);
        statCore.registerStat(StatType.LEVEL, level);
        statCore.registerStat(StatType.MAX_EXP, maxExp);
    }
}
