using UnityEngine;

/// <summary>
/// 포식의 마법: 적 처치 시 배고픔을 추가 회복한다.
/// 효과는 EnemyEvents.OnDeath 이벤트를 통해서만 적용된다.
/// </summary>
public class PredationMagic : MonoBehaviour
{
    [Tooltip("적 처치 시 회복할 배고픔 수치")]
    public float hungerRestoreAmount = 10f;

    private PlayerStatCore statCore;

    private void Start()
    {
        statCore = GetComponent<PlayerStatManager>().StatCore;
    }

    private void OnEnable()
    {
        EnemyEvents.OnDeath += OnEnemyDeath;
    }

    private void OnDisable()
    {
        EnemyEvents.OnDeath -= OnEnemyDeath;
    }

    public void OnEnemyDeath(EnemyDeathEvent e)
    {
        if (statCore == null) return;

        float currentHunger = statCore.getStat(StatType.HUNGER).rawValue;
        float maxHunger     = statCore.getStat(StatType.MAX_HUNGER).rawValue;
        float newHunger     = Mathf.Min(currentHunger + hungerRestoreAmount, maxHunger);

        statCore.registerStat(StatType.HUNGER, newHunger);
        Debug.Log($"[PredationMagic] 배고픔 회복 {currentHunger:0.#} → {newHunger:0.#} (max {maxHunger:0.#})");
    }
}
