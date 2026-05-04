using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Skill : MonoBehaviour
{
    public Image cooldownBar;
    public TextMeshProUGUI skillText;

    public SkillData fireballData;
    public SkillData dashData;
    public SkillData roarData;

    public GameObject fireballPrefab;
    public GameObject roarEffectPrefab;

    private Rigidbody2D rb;

    private Dictionary<SkillType, float> lastUsedTime = new Dictionary<SkillType, float>();

    private SkillData currentSkill;
    private bool isDashing = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        lastUsedTime[SkillType.Fireball] = -999f;
        lastUsedTime[SkillType.Dash] = -999f;
        lastUsedTime[SkillType.Roar] = -999f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            UseSkill(fireballData);

        if (Input.GetKeyDown(KeyCode.Q))
            UseSkill(dashData);

        if (Input.GetKeyDown(KeyCode.W))
            UseSkill(roarData);

        UpdateUI();
    }

    void UseSkill(SkillData data)
{
    SkillType type = data.skillType;

    if (Time.time < lastUsedTime[type] + data.cooldown)
    {
        Debug.Log("쿨타임 중!");
        return;
    }

    lastUsedTime[type] = Time.time;
    currentSkill = data;

    Debug.Log("스킬 사용: " + data.skillName);

    if (data.effectType == SkillEffectType.Buff)
    {
        Debug.Log("버프 스킬");
    }
    else if (data.effectType == SkillEffectType.Debuff)
    {
        Debug.Log("디버프 스킬");
    }

    switch (type)
    {
        case SkillType.Fireball:
            Fireball();
            break;

        case SkillType.Dash:
            StartCoroutine(Dash());
            break;

        case SkillType.Roar:
            Roar();
            break;
    }
}

    void Fireball()
    {
        Vector3 spawnPos = transform.position + transform.right * 2f;
        Instantiate(fireballPrefab, spawnPos, Quaternion.identity);
    }

    IEnumerator Dash()
{
    float dashSpeed = 25f;
    float dashTime = 0.15f;
    float hitRadius = 1.5f;

    float startTime = Time.time;
    Vector2 dir = transform.right;

    while (Time.time < startTime + dashTime)
    {
        rb.linearVelocity = dir * dashSpeed;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                if (currentSkill.effectType == SkillEffectType.Debuff)
                {
                    Enemy enemy = hit.GetComponent<Enemy>();

                    if (enemy != null)
                    {
                        // enemy.ApplySlow(2f, 0.5f);
                    }
                }
            }
        }

        yield return null;
    }

    rb.linearVelocity = Vector2.zero;
}

    void Roar()
{
    float radius = 3f;

    Debug.Log("포효!!!");

    GameObject fx = Instantiate(roarEffectPrefab, transform.position, Quaternion.identity);

    Destroy(fx, 2f);

    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

    foreach (Collider2D hit in hits)
    {
        if (hit.CompareTag("Enemy"))
        {
            Debug.Log("포효 데미지!");

            if (currentSkill.effectType == SkillEffectType.Debuff)
            {
                Enemy enemy = hit.GetComponent<Enemy>();

                if (enemy != null)
                {
                    // enemy.ApplySlow(2f, 0.5f);
                }
            }
        }
    }
}

    void UpdateUI()
    {
        if (currentSkill == null) return;

        SkillType type = currentSkill.skillType;

        float lastTime = lastUsedTime[type];
        float remaining = (lastTime + currentSkill.cooldown) - Time.time;

        if (remaining < 0) remaining = 0;

        float ratio = 1 - (remaining / currentSkill.cooldown);

        cooldownBar.fillAmount = ratio;

        skillText.text =
            currentSkill.skillName + "\n" +
            "데미지: " + currentSkill.damage + "\n" +
            "쿨타임: " + remaining.ToString("F1");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }

    void OnCollisionEnter2D(Collision2D collision)
{
    if (isDashing && currentSkill.effectType == SkillEffectType.Debuff)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            // if (enemy != null)
            // {
            //     enemy.ApplySlow(2f, 0.5f);
            //     Debug.Log("대시 디버프 적용!");
            // }
        }
    }
}

}