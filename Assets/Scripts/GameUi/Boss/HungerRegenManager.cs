using UnityEngine;

public class HungerRegenManager : MonoBehaviour
{
    [Header("플레이어 자동 탐색")]
    [SerializeField] private PlayerStatManager playerStatManager;

    [Header("배고픔 회복 설정")]
    [SerializeField] private float regenInterval = 3f;
    [SerializeField] private float regenAmount = 1f;

    private PlayerStatCore statCore;
    private float timer;

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (statCore == null)
        {
            FindPlayer();
            return;
        }

        timer += Time.deltaTime;

        if (timer >= regenInterval)
        {
            timer = 0f;
            RegenHunger();
        }
    }

    private void FindPlayer()
    {
        if (playerStatManager == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
                playerStatManager = playerObj.GetComponent<PlayerStatManager>();
        }

        if (playerStatManager == null)
            return;

        if (playerStatManager.StatCore == null)
            return;

        statCore = playerStatManager.StatCore;
    }

    private void RegenHunger()
    {
        float currentHunger = statCore.getStat(StatType.HUNGER).rawValue;
        float maxHunger = statCore.getStat(StatType.MAX_HUNGER).rawValue;

        if (currentHunger >= maxHunger)
            return;

        float addAmount = Mathf.Min(regenAmount, maxHunger - currentHunger);

        statCore.addStat(StatType.HUNGER, addAmount);

        playerStatManager.playerHunger = statCore.getStat(StatType.HUNGER).rawValue;
    }
}