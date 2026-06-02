using UnityEngine;

[CreateAssetMenu(fileName = "FireballSkillData", menuName = "SkillTree/Actions/FireballSkillData")]
public class FireballSkillData : ActiveAction
{
    [Header("Fireball Settings")]
    public GameObject fireballPrefab;

    public float speed = 10f;

    public Vector3 spawnOffset = new Vector3(1f, 0f, 0f);

    public override void Process(GameObject player, ActiveSkillData data)
    {
        Debug.Log("파이어볼 Process 실행!");

        if (fireballPrefab == null || player == null)
            return;

        float direction =
            player.transform.localScale.x >= 0 ? 1f : -1f;

        Vector3 spawnPos =
            player.transform.position +
            new Vector3(
                spawnOffset.x * direction,
                spawnOffset.y,
                spawnOffset.z
            );

        GameObject fireball =
            Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

        Vector3 scale = fireball.transform.localScale;

        scale.x = Mathf.Abs(scale.x) * direction;
        fireball.transform.localScale = scale;

        PlayerStatManager statManager =
            player.GetComponent<PlayerStatManager>();

        if (statManager == null)
            return;

        float damage =
            statManager.StatCore
            .getStat(StatType.SKILL_DAMAGE)
            .rawValue;

        if (fireball.TryGetComponent<FireballProjectile>(out var projectile))
        {
            projectile.Initialize(direction, speed, damage);
        }
    }

    public override void Clear(GameObject player)
    {

    }
}