using UnityEngine;

public class SkillsBasicattack : MonoBehaviour
{
    private const float Damage = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponent<EnemyHp>().TakeDamage(Damage);
    }
}
