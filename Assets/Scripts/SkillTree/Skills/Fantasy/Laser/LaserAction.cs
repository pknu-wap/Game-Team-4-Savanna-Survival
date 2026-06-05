using UnityEngine;

[CreateAssetMenu(fileName = "Laser", menuName = "SkillTree/Actions/Laser")]
public class LaserAction : ActiveAction
{
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float hungerCost = 5f;
    [SerializeField] private float baseDamage = 0f;
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float spawnOffset = 0.5f;
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private float vfxDuration = 0.2f;

    public override bool CanProcess(GameObject player, ActiveSkillData data)
    {
        PlayerStatCore statCore = player.GetComponent<PlayerStatManager>().StatCore;
        return statCore.getStat(StatType.HUNGER).rawValue >= hungerCost;
    }

    public override void Process(GameObject player, ActiveSkillData data)
    {
        PlayerStatCore statCore = player.GetComponent<PlayerStatManager>().StatCore;
        float currentHunger = statCore.getStat(StatType.HUNGER).rawValue;
        if (currentHunger < hungerCost)
        {
            Debug.Log("레이저: 허기 부족");
            return;
        }

        statCore.registerStat(StatType.HUNGER, currentHunger - hungerCost);

        Vector2 direction = player.GetComponent<PlayerMovement>().GetLastMoveDirection();
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            direction = direction.x >= 0f ? Vector2.right : Vector2.left;
        else
            direction = direction.y >= 0f ? Vector2.up : Vector2.down;

        Vector3 spawnPosition = player.transform.position + (Vector3)(direction * spawnOffset);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject laser = Instantiate(laserPrefab, spawnPosition, Quaternion.Euler(0f, 0f, angle));

        float damage = Mathf.Max(1f,
            baseDamage + statCore.getStat(StatType.SKILL_DAMAGE).calibratedValue * damageMultiplier);
        laser.AddComponent<LaserBeamController>().Init(damage, duration, vfxPrefab, vfxDuration);
    }

    public override void Clear(GameObject player)
    {

    }
}
