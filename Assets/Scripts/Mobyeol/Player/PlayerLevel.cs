using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public PlayerStatCore statCore;
    private float currentExp;
    private float maxExp;
    private float level;

    void Start()
    {
        PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
        statCore = playerStatManager.StatCore;
    }

    public void addExp(float value)
    {
        currentExp = statCore.getStat(StatType.EXP).rawValue;
        maxExp = statCore.getStat(StatType.MAX_EXP).rawValue;
        level = statCore.getStat(StatType.LEVEL).rawValue;

        currentExp += value;

        while (currentExp >= maxExp)
        {
            currentExp -= maxExp;
            level++;
            maxExp *= 1.2f; //임시 레벨업 경험치통 증가 수치
            // 레벨업 이벤트 호출
            Debug.Log("레벨업! 현재 레벨: " + level);
        }

        statCore.registerStat(StatType.EXP, currentExp);
        statCore.registerStat(StatType.LEVEL, level);
        statCore.registerStat(StatType.MAX_EXP, maxExp);
    }
}
