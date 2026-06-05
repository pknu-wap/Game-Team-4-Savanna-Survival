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
        // ✅ 보스는 허기 체크 없이 항상 사용 가능
        if (player.GetComponent<BossLaserController>() != null) return true;

        var statCore = player.GetComponent<PlayerStatManager>()?.StatCore;
        if (statCore == null) return false;
        return statCore.getStat(StatType.HUNGER).rawValue >= hungerCost;
    }

    public override void Process(GameObject player, ActiveSkillData data)
    {
        var statCore = player.GetComponent<PlayerStatManager>()?.StatCore;
        if (statCore == null) return;

        // ✅ 보스 여부 확인
        var bossCtrl = player.GetComponent<BossLaserController>();
        bool isBoss  = bossCtrl != null;

        // 허기 체크 — 보스는 스킵
        if (!isBoss)
        {
            float currentHunger = statCore.getStat(StatType.HUNGER).rawValue;
            if (currentHunger < hungerCost)
            {
                Debug.Log("레이저: 허기 부족");
                return;
            }
            statCore.registerStat(StatType.HUNGER, currentHunger - hungerCost);
        }

        // 발사 방향 — PlayerMovement / BossMovementBridge 둘 다 PlayerMovement 상속이므로 통일
        var movement  = player.GetComponent<PlayerMovement>();
        Vector2 direction = movement != null ? movement.GetLastMoveDirection() : Vector2.right;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            direction = direction.x >= 0f ? Vector2.right : Vector2.left;
        else
            direction = direction.y >= 0f ? Vector2.up : Vector2.down;

        Vector3 spawnPosition = player.transform.position + (Vector3)(direction * spawnOffset);
        float   angle         = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject laser      = Instantiate(laserPrefab, spawnPosition,
                                            Quaternion.Euler(0f, 0f, angle));

        float damage = Mathf.Max(1f,
            baseDamage + statCore.getStat(StatType.SKILL_DAMAGE).calibratedValue * damageMultiplier);

        // ✅ 보스면 Player 레이어 타격, 플레이어면 Enemy 레이어 타격
        int targetMask = isBoss
            ? (int)bossCtrl.TargetLayer
            : LayerMask.GetMask("Enemy");

        laser.AddComponent<LaserBeamController>()
             .Init(damage, duration, vfxPrefab, vfxDuration, targetMask, isBoss);
    }

    public override void Clear(GameObject player) { }
}