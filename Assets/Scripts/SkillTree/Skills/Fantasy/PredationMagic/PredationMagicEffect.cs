using UnityEngine;

/// <summary>
/// 포식의 마법 패시브 효과. 플레이어에 PredationMagic 컴포넌트를 부착한다.
/// </summary>
[CreateAssetMenu(menuName = "SkillTree/Effects/PredationMagicEffect")]
public class PredationMagicEffect : PassiveEffect
{
    [Tooltip("적 처치 시 회복할 배고픔 수치")]
    public float hungerRestoreAmount = 10f;

    public override void Apply(GameObject player)
    {
        var passive = player.GetComponent<PredationMagic>();
        if (passive == null)
            passive = player.AddComponent<PredationMagic>();

        passive.hungerRestoreAmount = hungerRestoreAmount;
    }

    public override void Remove(GameObject player)
    {
        var passive = player.GetComponent<PredationMagic>();
        if (passive != null)
            Object.Destroy(passive);
    }
}
