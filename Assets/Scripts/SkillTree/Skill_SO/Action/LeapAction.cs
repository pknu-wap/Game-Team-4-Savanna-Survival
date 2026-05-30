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
    [SerializeField] private LayerMask enemyLayers;

    private readonly Dictionary<GameObject, Coroutine> activeRoutines = new();

    public override void Process(GameObject player, ActiveSkillData data)
    {
        if (player == null) return;

        PlayerSkillController controller = player.GetComponent<PlayerSkillController>();
        if (controller == null) return;

        StopCurrent(player, controller);
        Coroutine routine = controller.StartCoroutine(LeapRoutine(player, CalculateDamage(player)));
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

    private IEnumerator LeapRoutine(GameObject player, float damage)
    {
        Vector2 direction = GetDirection(player);
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.stopMove();

        Vector2 startPosition = player.transform.position;
        Vector2 endPosition = startPosition + direction * distance;

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

        if (movement != null)
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
            damagedEnemies.Add(enemy);
        }
    }
}
