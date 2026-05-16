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
    public GameObject skillUI;

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

        if (skillUI != null)
            skillUI.SetActive(false);
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
            return;

        lastUsedTime[type] = Time.time;
        currentSkill = data;

        if (skillUI != null)
        {
            skillUI.SetActive(true);
            StartCoroutine(HideUI());
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

    IEnumerator HideUI()
    {
        yield return new WaitForSeconds(2f);
        if (skillUI != null)
            skillUI.SetActive(false);
    }

    void Fireball()
    {
        Vector3 spawnPos = transform.position + transform.right * 2f;
        Instantiate(fireballPrefab, spawnPos, transform.rotation);
    }

    IEnumerator Dash()
    {
        float dashSpeed = 25f;
        float dashTime = 0.15f;
        float hitRadius = 1.5f;

        isDashing = true;

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
        isDashing = false;
    }

    void Roar()
{
    float radius = 5f;

    GameObject fx = Instantiate(roarEffectPrefab, transform.position, Quaternion.identity);

    fx.transform.SetParent(transform);

    Destroy(fx, 2f);

    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

    foreach (Collider2D hit in hits)
    {
        if (hit.CompareTag("Enemy"))
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy != null)
            {
                // enemy.ApplySlow(2f, 0.5f);
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
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}