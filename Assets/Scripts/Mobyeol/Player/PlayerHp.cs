using UnityEngine;

public class PlayerHp : MonoBehaviour
{
    private PlayerStatCore statCore;
    private float currntHp;
    private float maxHp;
    private float hpRegen;
    private float currntTime;

    private void Start()
    {
        PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
        statCore = playerStatManager.StatCore;
    }

    private void Update()
    {
        maxHp = statCore.getStat(StatType.MAX_HEALTH).rawValue;
        currntHp = statCore.getStat(StatType.HEALTH).rawValue;
        hpRegen = statCore.getStat(StatType.HEALTH_REGEN).rawValue;
        currntTime += Time.deltaTime;

        if(currntHp <= 0)
        {
            Debug.Log("죽었습니다.");
            //사망시 event 호출
        }
        if(currntTime >= 2)
        {
            if (currntHp + hpRegen*2 >= maxHp)
            {
               currntHp = maxHp;
            }
            else
            {
                currntHp += hpRegen * 2; 
            }
            statCore.registerStat(StatType.HEALTH, currntHp);
            currntTime = 0;
            Debug.Log("현재 체력: " + currntHp);
        }

    }
}
