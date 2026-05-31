using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TornadoAction", menuName = "SkillTree/Actions/TornadoAction")]
public class TornadoAction : ActiveAction
{
    [SerializeField] private GameObject tornadoPrefab;
    [SerializeField] private float damageTick = 0.5f;
    [SerializeField] private float baseDamage = 1f;
    [SerializeField] private float pullDistancePerSecond = 2f;
    [SerializeField] private float duration = 3f;
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private float vfxDuration = 0.2f;
    [SerializeField] private LayerMask enemyLayers;

    private readonly Dictionary<GameObject, Coroutine> activeRoutines = new();
    private readonly Dictionary<GameObject, GameObject> activeTornadoes = new();

    public override void Process(GameObject player, ActiveSkillData data)
    {
        var controller = player.GetComponent<PlayerSkillController>();
        StopCurrent(player, controller);

        activeRoutines[player] = controller.StartCoroutine(
            TornadoRoutine(player, CalculateDamage(player)));
    }

    public override void Clear(GameObject player)
    {
        StopCurrent(player, player.GetComponent<PlayerSkillController>());
    }

    private float CalculateDamage(GameObject player)
    {
        return Mathf.Max(1f, player.GetComponent<PlayerStatManager>().StatCore
            .getStat(StatType.SKILL_DAMAGE).calibratedValue * baseDamage);
    }

    private LayerMask GetEnemyLayers()
    {
        return enemyLayers.value == 0 ? LayerMask.GetMask("Enemy") : enemyLayers;
    }

    private void StopCurrent(GameObject player, PlayerSkillController controller)
    {
        if (activeRoutines.TryGetValue(player, out Coroutine routine))
        {
            activeRoutines.Remove(player);
            if (routine != null)
                controller.StopCoroutine(routine);
        }

        if (!activeTornadoes.TryGetValue(player, out GameObject tornado)) return;

        activeTornadoes.Remove(player);
        if (tornado != null)
            Object.Destroy(tornado);
    }

    private IEnumerator TornadoRoutine(GameObject player, float damage)
    {
        GameObject tornado = Instantiate(tornadoPrefab, player.transform.position, Quaternion.identity);
        activeTornadoes[player] = tornado;

        Collider2D area = tornado.GetComponent<Collider2D>();
        ContactFilter2D enemyFilter = new();
        enemyFilter.SetLayerMask(GetEnemyLayers());
        enemyFilter.useTriggers = true;

        Collider2D[] hitBuffer = new Collider2D[64];
        HashSet<Enemy> pulledEnemies = new();

        float elapsed = 0f;
        float tickTimer = damageTick;
        while (elapsed < duration && tornado != null)
        {
            elapsed += Time.deltaTime;
            tickTimer += Time.deltaTime;

            bool shouldDamage = tickTimer >= damageTick;
            if (shouldDamage)
                tickTimer = 0f;

            pulledEnemies.Clear();

            int hitCount = area.Overlap(enemyFilter, hitBuffer);

            Vector2 center = tornado.transform.position;
            for (int i = 0; i < hitCount; i++)
            {
                Enemy enemy = hitBuffer[i].GetComponentInParent<Enemy>();
                if (enemy == null || !pulledEnemies.Add(enemy)) continue;

                PullEnemy(enemy, center);

                if (shouldDamage)
                    DamageEnemy(enemy, damage);
            }

            yield return null;
        }

        activeRoutines.Remove(player);
        activeTornadoes.Remove(player);
        if (tornado != null)
            Object.Destroy(tornado);
    }

    private void PullEnemy(Enemy enemy, Vector2 center)
    {
        Rigidbody2D enemyBody = enemy.GetComponent<Rigidbody2D>();
        Vector2 currentPosition = enemyBody != null
            ? enemyBody.position
            : enemy.transform.position;
        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            center,
            pullDistancePerSecond * Time.deltaTime);

        if (enemyBody != null)
            enemyBody.MovePosition(nextPosition);
        else
            enemy.transform.position = nextPosition;
    }

    private void DamageEnemy(Enemy enemy, float damage)
    {
        enemy.TakeDamage(damage);
        if (vfxPrefab == null) return;

        var vfx = Instantiate(vfxPrefab, enemy.transform.position, Quaternion.identity);
        Object.Destroy(vfx, vfxDuration);
    }
}
