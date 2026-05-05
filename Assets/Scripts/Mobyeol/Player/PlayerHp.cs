using UnityEngine;

public class PlayerHp : MonoBehaviour
{
    private PlayerStatCore statCore;
    private float currentHp;
    private float maxHp;
    private float hpRegen;
    private float currentTime;

    private Animator animator;
    private PlayerMovement playerMovement;

    private bool isDead = false;

    private void Start()
    {
        PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
        statCore = playerStatManager.StatCore;

        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (isDead) return;

        maxHp = statCore.getStat(StatType.MAX_HEALTH).rawValue;
        currentHp = statCore.getStat(StatType.HEALTH).rawValue;
        hpRegen = statCore.getStat(StatType.HEALTH_REGEN).rawValue;
        currentTime += Time.deltaTime;

        if(currentHp <= 0)
        {
            onDeath();
            return;
        }
        if(currentTime >= 2)
        {
            if (currentHp + hpRegen*2 >= maxHp)
            {
               currentHp = maxHp;
            }
            else
            {
                currentHp += hpRegen * 2; 
            }
            statCore.registerStat(StatType.HEALTH, currentHp);
            currentTime = 0;
            Debug.Log("현재 체력: " + currentHp);
        }
    }

    private void onDeath()
    {
        isDead = true;

        currentHp = 0;
        statCore.registerStat(StatType.HEALTH, currentHp);

        animator.SetBool("isMoving", false);
        animator.SetBool("isDead", true);

        if (playerMovement != null)
        {
            playerMovement.stopMove();
            playerMovement.enabled = false;
        }

        Debug.Log("죽었습니다.");

        // 사망 event 호출
    }
}