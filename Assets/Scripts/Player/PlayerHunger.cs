using UnityEngine;

public class PlayerHunger : MonoBehaviour
{
    public event System.Action OnHungerFilled;

    private PlayerStatCore statCore;
    private float currentHunger;
    private float maxHunger;
    private float hungerDecraesePerSec;
    public bool isHungerDebuff;
    private float currntTime;

    [SerializeField] private PlayerHp playerHp;

    void Start()
    {
        PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>(); 
        statCore = playerStatManager.StatCore;
        maxHunger = statCore.getStat(StatType.MAX_HUNGER).rawValue;
        currentHunger = statCore.getStat(StatType.HUNGER).rawValue;
        hungerDecraesePerSec = statCore.getStat(StatType.HUNGER_DECREASE).rawValue;
    }

    void Update()
    {
        currntTime += Time.deltaTime;
        
        if (currntTime >= 1)
        {
            maxHunger = statCore.getStat(StatType.MAX_HUNGER).rawValue;
            currentHunger = statCore.getStat(StatType.HUNGER).rawValue;
            hungerDecraesePerSec = statCore.getStat(StatType.HUNGER_DECREASE).rawValue;

            currntTime = 0;

            if (currentHunger >= maxHunger)
            {
                OnHungerFilled?.Invoke();
                hungerBuff();
                currentHunger = maxHunger * 0.6f;
            }
            else if (currentHunger <= 1)
            {
                currentHunger = 0;
            }
            else
            {
                currentHunger -= hungerDecraesePerSec;
            }
        
            statCore.registerStat(StatType.HUNGER, currentHunger);

            if (currentHunger < maxHunger * 0.2f)
            {
                hungerDebuffApply();
                isHungerDebuff = true;
            }
            else 
                isHungerDebuff = false;
        }
    }

    private void hungerBuff()
    {
        float maxHp = playerHp.maxHp;
        float hp = playerHp.currentHp;

        if (maxHp <= hp + maxHp * 0.3)
        {
            statCore.registerStat(StatType.HEALTH, maxHp);
        }
        else
        {
            statCore.addStat(StatType.HEALTH, maxHp*0.3f);
        }
    }

    private void hungerDebuffApply()
    {
        statCore.addStat(StatType.HEALTH, -playerHp.maxHp * 0.03f);
    }
}
