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

    [Header("범위 스프라이트")]
    [SerializeField] private Sprite rangeSprite;
    [SerializeField] private Color rangeColor = new Color(1f, 0f, 0f, 0.25f);
    [SerializeField] private float spriteOnTime = 0.15f;

    private PlayerStatCore statCore;
    private GameObject rangeObject;
    private SpriteRenderer rangeRenderer;

    private float attackTimer;

    private float attackDamageBonus = 0f;
    private float attackRangeBonus = 0f;

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

        CreateRangeSprite();
        UpdateRangeSpriteSize();
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
        StartCoroutine(ShowRangeSprite());

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, FinalAttackRadius);

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy == null) continue;

            float damage =
                statCore.getStat(StatType.DAMAGE).calibratedValue
                * (damageConstant + attackDamageBonus)
                / damageCorrection;

            enemy.TakeDamage(damage);
        }
    }

    private void CreateRangeSprite()
    {
        rangeObject = new GameObject("Basic Attack Range Sprite");
        rangeObject.transform.SetParent(transform);
        rangeObject.transform.localPosition = Vector3.zero;

        rangeRenderer = rangeObject.AddComponent<SpriteRenderer>();
        rangeRenderer.sprite = rangeSprite;
        rangeRenderer.color = rangeColor;
        rangeRenderer.sortingOrder = -1;

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
}