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
        activeRoutines[player] = controller.StartCoroutine(
            LeapRoutine(player, CalculateDamage(player)));
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

    private LayerMask GetTargetLayers(GameObject player)
    {
        var bossCtrl = player.GetComponent<BossWildController>();
        if (bossCtrl != null) return bossCtrl.TargetLayer;
        return enemyLayers.value == 0 ? LayerMask.GetMask("Enemy") : enemyLayers;
    }

    private void StopCurrent(GameObject player, PlayerSkillController controller)
    {
        if (!activeRoutines.TryGetValue(player, out Coroutine routine)) return;
        activeRoutines.Remove(player);
        if (routine != null) controller.StopCoroutine(routine);
        player.GetComponent<PlayerMovement>().reStartMove();
    }

    private IEnumerator LeapRoutine(GameObject player, float damage)
    {
        var movement = player.GetComponent<PlayerMovement>();
        movement.stopMove();

        Vector2 startPosition = player.transform.position;
        Vector2 endPosition   = startPosition + movement.GetLastMoveDirection() * distance;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float ratio      = Mathf.Clamp01(elapsed / duration);
            float easedRatio = Mathf.SmoothStep(0f, 1f, ratio);
            player.transform.position = Vector2.Lerp(startPosition, endPosition, easedRatio);
            yield return null;
        }

        player.transform.position = endPosition;
        DamageLandingArea(player, endPosition, damage);
        movement.reStartMove();
        activeRoutines.Remove(player);
    }

    private void DamageLandingArea(GameObject player, Vector2 center, float damage)
    {
        var bossCtrl  = player.GetComponent<BossWildController>();
        bool isBoss   = bossCtrl != null;
        int  mask     = (int)GetTargetLayers(player);

        var hits = Physics2D.OverlapCircleAll(center, landingRadius, mask);
        HashSet<object> damaged = new();

        foreach (var hit in hits)
        {
            if (isBoss)
            {
                // ✅ 플레이어 타격 — 중복 방지
                var playerEffect = hit.GetComponent<PlayerEffectTemp>();
                var statManager  = hit.GetComponent<PlayerStatManager>();
                object key       = (object)playerEffect ?? statManager;
                if (key == null || !damaged.Add(key)) continue;

                bossCtrl.DamagePlayer(hit, damage);
            }
            else
            {
                var enemy = hit.GetComponentInParent<Enemy>();
                if (enemy == null || !damaged.Add(enemy)) continue;
                enemy.TakeDamage(damage);
            }

            if (vfxPrefab != null)
            {
                var vfx = Instantiate(vfxPrefab, hit.transform.position, Quaternion.identity);
                Object.Destroy(vfx, vfxDuration);
            }
        }
    }
}
