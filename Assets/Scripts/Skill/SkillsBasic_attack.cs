using UnityEngine;
using System.Collections.Generic;

public class SkillsBasic_attack : MonoBehaviour
{
    [Header("데미지 설정")]
    [SerializeField] private float damageConstant = 10f;
    [SerializeField] private float damageCorrection = 3.5f;

    [Header("공격 범위 / 주기 설정")]
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private float attackInterval = 1f;

    private PlayerStatCore statCore;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;
    private float attackTimer = 0f;

    // 이중 타격 방지: 펄스 1회당 맞은 적 목록
    private readonly HashSet<Enemy> hitSet = new HashSet<Enemy>();

    private void Start()
    {
        PlayerStatManager playerStatManager = GetComponentInParent<PlayerStatManager>();
        if (playerStatManager == null)
        {
            Debug.LogError("PlayerStatManager를 찾을 수 없습니다.");
            return;
        }
        statCore = playerStatManager.StatCore;

        circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider == null)
            circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;

        // 스케일 영향 제거: 항상 (1,1,1) 유지
        transform.localScale = Vector3.one;

        SetupVisual();
        ApplyRadius();
    }

    private void SetupVisual()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        if (spriteRenderer.sprite == null)
            spriteRenderer.sprite = CreateCircleSprite();

        spriteRenderer.color = new Color(1f, 0f, 0f, 0.15f);
        spriteRenderer.sortingOrder = -1;
    }

    private Sprite CreateCircleSprite()
    {
        int resolution = 128;
        Texture2D tex = new Texture2D(resolution, resolution);
        Vector2 center = new Vector2(resolution / 2f, resolution / 2f);
        float radius = resolution / 2f;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01(1f - (dist - (radius - 2f)) / 2f);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, dist <= radius ? alpha : 0f));
            }
        }
        tex.Apply();

        // PPU = 1로 설정 → 월드 크기 = resolution 유닛
        // localScale로 attackRadius * 2 크기가 되도록 조정
        return Sprite.Create(
            tex,
            new Rect(0, 0, resolution, resolution),
            new Vector2(0.5f, 0.5f),
            1f // PPU = 1 → 스프라이트 자체 크기 = resolution 유닛
        );
    }

    private void ApplyRadius()
    {
        // localScale로 스프라이트 크기 조정 (PPU=1이므로 diameter 유닛)
        float diameter = attackRadius * 2f;
        transform.localScale = new Vector3(diameter, diameter, 1f);

        // 콜라이더는 localScale 영향을 받으므로:
        // 실제 월드 반지름 = collider.radius * localScale.x
        // 원하는 값: attackRadius = col.radius * diameter
        // → col.radius = attackRadius / diameter = 0.5f
        if (circleCollider != null)
            circleCollider.radius = 0.5f;
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            StartCoroutine(AttackPulse());
        }
    }

    private System.Collections.IEnumerator AttackPulse()
    {
        // 펄스 시작 시 이중 타격 방지용 집합 초기화
        hitSet.Clear();

        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.4f);

        // 콜라이더를 한 프레임 껐다 켜서 Trigger 재발동
        circleCollider.enabled = false;
        yield return null;
        circleCollider.enabled = true;

        yield return new WaitForSeconds(0.1f);

        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.15f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (statCore == null) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null)
            enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        // 이미 이번 펄스에서 맞은 적이면 무시
        if (!hitSet.Add(enemy)) return;

        float damage =
            statCore.getStat(StatType.DAMAGE).calibratedValue *
            damageConstant /
            damageCorrection;

        enemy.TakeDamage(damage);
    }

    public void AddAttackDamageBonus(float value) => damageConstant += value;

    public void AddAttackRangeBonus(float value)
    {
        attackRadius = Mathf.Max(0.1f, attackRadius + value);
        ApplyRadius();
    }

    public void AddAttackIntervalBonus(float value)
    {
        attackInterval = Mathf.Max(0.1f, attackInterval + value);
    }

    public void AddDamageCorrectionBonus(float value)
    {
        damageCorrection = Mathf.Max(0.1f, damageCorrection + value);
    }

#if UNITY_EDITOR
    private float prevRadius = -1f;

    private void OnValidate()
    {
        if (Mathf.Approximately(attackRadius, prevRadius)) return;
        prevRadius = attackRadius;

        float diameter = attackRadius * 2f;

        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null) return;
            transform.localScale = new Vector3(diameter, diameter, 1f);

            var col = GetComponent<CircleCollider2D>();
            if (col != null) col.radius = 0.5f;
        };
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
#endif
}