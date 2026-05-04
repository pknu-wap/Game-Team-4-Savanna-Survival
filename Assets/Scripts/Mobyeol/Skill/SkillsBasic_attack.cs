using UnityEngine;

public class SkillsBasic_attack : MonoBehaviour
{
    private const float value = 10f; //스킬 데미지
    private PlayerStatCore statCore;

    private void Start()
    {
        PlayerStatManager playerStatManager = GetComponentInParent<PlayerStatManager>();
        statCore = playerStatManager.StatCore;
    }

    private void OnTriggerEnter2D(Collider2D other) //충돌시 데미지 구현
    {
        float damage = statCore.getStat(StatType.DAMAGE).calibratedValue * value / 3.5f; //임시
        other.GetComponent<EnemyHp>().takeDamage(damage);
    }
}
