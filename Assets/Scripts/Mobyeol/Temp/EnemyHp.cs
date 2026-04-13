using UnityEngine;

public class EnemyHp : MonoBehaviour
{
    //일반공격을 구현 및 실행해보기위해 스터디의 적 체력 스크립트를 가져왔습니다.
    private const float MaxHp = 100f;
    private float currentHp = MaxHp;

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        Debug.Log("적 체력: " + currentHp);
        if (currentHp <= 0f) Destroy(gameObject);
    }
}