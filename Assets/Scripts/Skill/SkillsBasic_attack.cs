using UnityEngine;
using System.Collections;

public class SkillsBasic_attack : MonoBehaviour
{
    [Header("데미지 설정")]
    [SerializeField] private float damageConstant = 10f;
    [SerializeField] private float damageCorrection = 3.5f;

    [Header("공격 범위 / 주기")]
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private float attackInterval = 1f;

    //[Header("디버그")]
    //[SerializeField] private bool debugTriggerLog = true;

    private PlayerStatCore statCore;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;
    private float attackTimer = 0f;
    private bool isAttackActive = false;
    private int triggerLogCount = 0;

    private float FinalAttackRadius => Mathf.Max(0.1f, attackRadius + attackRangeBonus);

    private void Start()
    {
        PlayerStatManager playerStatManager = GetComponentInParent<PlayerStatManager>();

        if (playerStatManager == null)
        {
            Debug.LogError("PlayerStatManager를 찾지 못했습니다.");
            return;
        }

        statCore = playerStatManager.StatCore;

        SetupVisual();
        ApplyRadius();

        // 범위 표시는 항상 보이지만, 데미지 트리거는 공격 펄스 중에만 켠다.
        circleCollider.enabled = false;
    }

    private void Update()
    {
        if (statCore == null) return;

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            Attack();
        }
    }

    public void AddAttackDamageBonus(float value)
    {
        attackDamageBonus += value;
    }

    public void AddAttackRangeBonus(float value)
    {
        attackRangeBonus += value;
        UpdateRangeSpriteSize();
    }

    private void Attack()
    {
        // 펄스 시작 시 이중 타격 방지용 집합 초기화
        hitSet.Clear();
        isAttackActive = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, FinalAttackRadius);

        // 진한 빨강 펄스 중에만 Trigger 판정을 켠다.
        circleCollider.enabled = false;
        yield return null;
        circleCollider.enabled = true;

            float damage =
                statCore.getStat(StatType.DAMAGE).calibratedValue
                * (damageConstant + attackDamageBonus)
                / damageCorrection;

        circleCollider.enabled = false;
        isAttackActive = false;

        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.15f);
    }

    private void CreateRangeSprite()
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null)
            enemy = other.GetComponentInParent<Enemy>();

        bool alreadyHit = enemy != null && hitSet.Contains(enemy);

        /*
        if (debugTriggerLog)
        {
            Debug.Log(
                $"[BasicAttack Trigger #{++triggerLogCount}] " +
                $"active={isAttackActive}, " +
                $"other={other.name}, " +
                $"otherCollider={other.GetType().Name}, " +
                $"enemy={(enemy != null ? enemy.name : "None")}, " +
                $"alreadyHit={alreadyHit}, " +
                $"selfColliders={GetSelfColliderDebugInfo()}",
                this
            );
        }
        */

        if (!isAttackActive) return;
        if (statCore == null) return;
        if (enemy == null) return;

        // 이미 이번 펄스에서 맞은 적이면 무시
        if (!hitSet.Add(enemy)) return;

        float damage =
            statCore.getStat(StatType.DAMAGE).calibratedValue *
            damageConstant /
            damageCorrection;

        enemy.TakeDamage(damage);
    }

    private string GetSelfColliderDebugInfo()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        if (colliders.Length == 0) return "None";

        string result = "";
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider2D col = colliders[i];
            if (i > 0) result += ", ";
            result += $"{col.GetType().Name}(enabled={col.enabled}, trigger={col.isTrigger})";
        }

        return result;
    }

    public void AddAttackDamageBonus(float value) => damageConstant += value;

        rangeObject.SetActive(false);
    }

    private void UpdateRangeSpriteSize()
    {
        if (rangeObject == null) return;

        float diameter = FinalAttackRadius * 2f;
        rangeObject.transform.localScale = Vector3.one * diameter;
    }

    private IEnumerator ShowRangeSprite()
    {
        if (rangeObject == null) yield break;

        rangeObject.SetActive(true);
        yield return new WaitForSeconds(spriteOnTime);
        rangeObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        float radius = attackRadius + attackRangeBonus;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
