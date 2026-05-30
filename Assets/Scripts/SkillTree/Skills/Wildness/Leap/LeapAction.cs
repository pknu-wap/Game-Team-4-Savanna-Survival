using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Action_Leap", menuName = "SkillTree/Actions/Action_Leap")]
public class LeapAction : ActiveAction
{
    [SerializeField] private float distance = 5f;
    [SerializeField] private float duration = 0.32f;
    [SerializeField] private float landingRadius = 2f;
    [SerializeField] private float damageMultiplier = 1.6f;
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private float vfxDuration = 0.2f;
    [SerializeField] private LayerMask enemyLayers;

    private readonly Dictionary<GameObject, Coroutine> activeRoutines = new();

    public override void Process(GameObject player, ActiveSkillData data)
    {
        var controller = player.GetComponent<PlayerSkillController>();
        StopCurrent(player, controller);
        activeRoutines[player] = controller.StartCoroutine(LeapRoutine(player, CalculateDamage(player)));
    }

    public override void Clear(GameObject player)
    {
        StopCurrent(player, player.GetComponent<PlayerSkillController>());
    }

    private float CalculateDamage(GameObject player)
    {
        return Mathf.Max(1f, player.GetComponent<PlayerStatManager>().StatCore
            .getStat(StatType.SKILL_DAMAGE).calibratedValue * damageMultiplier);
    }

    private LayerMask GetEnemyLayers()
    {
        return enemyLayers.value == 0 ? LayerMask.GetMask("Enemy") : enemyLayers;
    }

    private void StopCurrent(GameObject player, PlayerSkillController controller)
    {
        if (!activeRoutines.TryGetValue(player, out Coroutine routine)) return;

        activeRoutines.Remove(player);
        if (routine != null)
            controller.StopCoroutine(routine);

        player.GetComponent<PlayerMovement>().reStartMove();
    }

    private IEnumerator LeapRoutine(GameObject player, float damage)
    {
        var movement = player.GetComponent<PlayerMovement>();
        movement.stopMove();

        Vector2 startPosition = player.transform.position;
        Vector2 endPosition = startPosition + movement.GetLastMoveDirection() * distance;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float ratio = Mathf.Clamp01(elapsed / duration);
            float easedRatio = Mathf.SmoothStep(0f, 1f, ratio);
            player.transform.position = Vector2.Lerp(startPosition, endPosition, easedRatio);
            yield return null;
        }

        player.transform.position = endPosition;
        DamageLandingArea(endPosition, damage);
        movement.reStartMove();
        activeRoutines.Remove(player);
    }

    private void DamageLandingArea(Vector2 center, float damage)
    {
        HashSet<Enemy> damagedEnemies = new();
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, landingRadius, GetEnemyLayers());
        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();
            if (enemy == null || damagedEnemies.Contains(enemy)) continue;

            enemy.TakeDamage(damage);
            if (vfxPrefab != null)
            {
                var vfx = Instantiate(vfxPrefab, enemy.transform.position, Quaternion.identity);
                Object.Destroy(vfx, vfxDuration);
            }
            damagedEnemies.Add(enemy);
        }
    }
}
