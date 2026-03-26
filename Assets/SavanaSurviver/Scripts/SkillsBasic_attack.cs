using UnityEngine;

public class SkillsBasic_attack : MonoBehaviour
{
    private const float Damage = 10f; //스킬 데미지

    private void OnTriggerEnter2D(Collider2D other) //충돌시 데미지 구현
    {
        other.GetComponent<EnemyHp>().TakeDamage(Damage);
    }
}
