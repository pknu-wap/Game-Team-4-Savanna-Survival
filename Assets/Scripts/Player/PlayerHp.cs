using UnityEngine;

public class PlayerHp : MonoBehaviour
{
    private PlayerStatCore  statCore;
    private Animator        animator;
    private PlayerMovement  playerMovement;

    internal float currentHp;
    internal float maxHp;
    private float  hpRegen;
    private float  currentTime;
    private bool   isDead;

    private void Start()
    {
        PlayerStatManager psm = GetComponent<PlayerStatManager>();
        if (psm == null)
        {
            Debug.LogError("[PlayerHp] PlayerStatManager 컴포넌트를 찾을 수 없습니다.", this);
            enabled = false;
            return;
        }
        statCore       = psm.StatCore;
        animator       = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (isDead || statCore == null) return;

        maxHp     = statCore.getStat(StatType.MAX_HEALTH).rawValue;
        currentHp = statCore.getStat(StatType.HEALTH).rawValue;
        hpRegen   = statCore.getStat(StatType.HEALTH_REGEN).rawValue;
        currentTime += Time.deltaTime;

        if (currentHp <= 0) { onDeath(); return; }

        if (currentTime >= 2)
        {
            currentHp = Mathf.Min(currentHp + hpRegen * 2, maxHp);
            statCore.registerStat(StatType.HEALTH, currentHp);
            currentTime = 0;
            Debug.Log("현재 체력: " + currentHp);
        }
    }

    private void onDeath()
    {
        isDead    = true;
        currentHp = 0;
        statCore.registerStat(StatType.HEALTH, currentHp);

        animator?.SetBool("isMoving", false);
        animator?.SetBool("isDead",   true);

        if (playerMovement != null)
        {
            playerMovement.stopMove();
            playerMovement.enabled = false;
        }

        Debug.Log("죽었습니다.");
        // 사망 event 호출
    }
}