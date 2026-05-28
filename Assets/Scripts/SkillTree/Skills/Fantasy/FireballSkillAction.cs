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

        if (fireball.TryGetComponent<FireballProjectile>(out var projectile))
        {
            projectile.Initialize(direction, speed);
        }
    }

    public override void Clear(GameObject player)
    {

    }
}
