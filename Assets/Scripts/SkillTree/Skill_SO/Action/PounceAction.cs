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
    [SerializeField] private LayerMask enemyLayers;

    private readonly Dictionary<GameObject, Coroutine> activeRoutines = new();

    public override void Process(GameObject player, ActiveSkillData data)
    {
        if (player == null) return;

        PlayerSkillController controller = player.GetComponent<PlayerSkillController>();
        if (controller == null) return;

        StopCurrent(player, controller);
        Coroutine routine = controller.StartCoroutine(PounceRoutine(player, CalculateDamage(player)));
        activeRoutines[player] = routine;
    }

    public override void Clear(GameObject player)
    {
        if (player == null) return;

        PlayerSkillController controller = player.GetComponent<PlayerSkillController>();
        if (controller != null)
            StopCurrent(player, controller);
    }

    private float CalculateDamage(GameObject player)
    {
        PlayerStatManager statManager = player.GetComponent<PlayerStatManager>();
        if (statManager == null) return damageMultiplier;

        float skillDamage = statManager.StatCore.getStat(StatType.SKILL_DAMAGE).rawValue;
        return Mathf.Max(1f, skillDamage * damageMultiplier);
    }

    private LayerMask GetEnemyLayers()
    {
        return enemyLayers.value == 0 ? LayerMask.GetMask("Enemy") : enemyLayers;
    }

    private Vector2 GetDirection(GameObject player)
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement == null) return Vector2.right;

        return movement.GetLastMoveDirection();
    }

    private void StopCurrent(GameObject player, PlayerSkillController controller)
    {
        if (activeRoutines.TryGetValue(player, out Coroutine routine))
        {
            controller.StopCoroutine(routine);
            activeRoutines.Remove(player);
        }

        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.reStartMove();
    }

    private IEnumerator PounceRoutine(GameObject player, float damage)
    {
        Vector2 direction = GetDirection(player);
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.stopMove();

        Vector2 startPosition = player.transform.position;
        Vector2 endPosition = startPosition + direction * distance;
        HashSet<Enemy> damagedEnemies = new();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float ratio = Mathf.Clamp01(elapsed / duration);
            player.transform.position = Vector2.Lerp(startPosition, endPosition, ratio);

            DamageEnemies(player.transform.position, damage, damagedEnemies);
            yield return null;
        }

        player.transform.position = endPosition;
        DamageEnemies(endPosition, damage, damagedEnemies);

        if (movement != null)
            movement.reStartMove();

        activeRoutines.Remove(player);
    }

    private void DamageEnemies(Vector2 center, float damage, HashSet<Enemy> damagedEnemies)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, hitRadius, GetEnemyLayers());
        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();
            if (enemy == null || damagedEnemies.Contains(enemy)) continue;

            enemy.TakeDamage(damage);
            damagedEnemies.Add(enemy);
        }
    }
}
