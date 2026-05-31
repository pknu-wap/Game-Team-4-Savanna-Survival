using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Action_Pounce", menuName = "SkillTree/Actions/Action_Pounce")]
public class PounceAction : ActiveAction
{
    [SerializeField] private float distance = 3f;
    [SerializeField] private float duration = 0.18f;
    [SerializeField] private float hitRadius = 0.8f;
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private float vfxDuration = 0.2f;
    [SerializeField] private LayerMask enemyLayers;

    private readonly Dictionary<GameObject, Coroutine> activeRoutines = new();

    public override void Process(GameObject player, ActiveSkillData data)
    {
        var controller = player.GetComponent<PlayerSkillController>();
        StopCurrent(player, controller);
        activeRoutines[player] = controller.StartCoroutine(PounceRoutine(player, CalculateDamage(player)));
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

    private IEnumerator PounceRoutine(GameObject player, float damage)
    {
        var movement = player.GetComponent<PlayerMovement>();
        movement.stopMove();

        Vector2 startPosition = player.transform.position;
        Vector2 direction = movement.GetLastMoveDirection();
        Vector2 endPosition = startPosition + direction * distance;
        Vector2 previousPosition = startPosition;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float ratio = Mathf.Clamp01(elapsed / duration);
            Vector2 nextPosition = Vector2.Lerp(startPosition, endPosition, ratio);

            if (TryHitEnemy(previousPosition, nextPosition, damage, out Vector2 stopPosition))
            {
                player.transform.position = stopPosition;
                movement.reStartMove();
                activeRoutines.Remove(player);
                yield break;
            }

            player.transform.position = nextPosition;
            previousPosition = nextPosition;
            yield return null;
        }

        player.transform.position = endPosition;
        movement.reStartMove();
        activeRoutines.Remove(player);
    }

    private bool TryHitEnemy(Vector2 from, Vector2 to, float damage, out Vector2 stopPosition)
    {
        Vector2 step = to - from;
        stopPosition = to;
        if (step.sqrMagnitude <= 0f) return false;

        RaycastHit2D hit = Physics2D.CircleCast(from, hitRadius, step.normalized, step.magnitude, GetEnemyLayers());
        if (hit.collider == null) return false;

        Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
        if (enemy == null) return false;

        stopPosition = hit.centroid;
        enemy.TakeDamage(damage);
        if (vfxPrefab != null)
        {
            var vfx = Instantiate(vfxPrefab, enemy.transform.position, Quaternion.identity);
            Object.Destroy(vfx, vfxDuration);
        }

        return true;
    }
}
