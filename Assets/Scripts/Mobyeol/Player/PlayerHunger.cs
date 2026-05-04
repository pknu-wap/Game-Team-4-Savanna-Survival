using UnityEngine;

public class PlayerHunger : MonoBehaviour
{
    private PlayerStatCore statCore;
    private float currentHunger;
    private float maxHunger;
    private float currntTime;
    
    void Start()
    {
        PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
        statCore = playerStatManager.StatCore;
    }

    void Update()
    {
        maxHunger = statCore.getStat(StatType.MAX_HUNGER).rawValue;
        currentHunger = statCore.getStat(StatType.HUNGER).rawValue;
        currntTime += Time.deltaTime;
        
        if (currntTime >= 1)
        {
            if (currentHunger <= 1)
            {
                currentHunger = 0;
            }
            else
            {
                currentHunger -= 1;
            }
            currntTime = 0;
            statCore.registerStat(StatType.HUNGER, currentHunger);
            // Debug.Log("허기감소");
        }
    }
}
