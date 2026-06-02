using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Effects/SatelliteEnhanceEffect")]
public class SatelliteEnhanceEffect : PassiveEffect
{
    [SerializeField] int countIncrease = 2;
    [SerializeField] float damageMultiplierBonus = 0.5f;

    public override void Apply(GameObject player)
    {
        var ctrl = player.GetComponent<SatelliteController>();
        if (ctrl == null) return;

        ctrl.maxSatelliteCount += countIncrease;
        ctrl.damageMultiplier += damageMultiplierBonus;
        ctrl.hasKnockback = true;
        ctrl.SpawnMissing(player.transform);
    }

    public override void Remove(GameObject player)
    {
        var ctrl = player.GetComponent<SatelliteController>();
        if (ctrl == null) return;

        ctrl.maxSatelliteCount -= countIncrease;
        ctrl.damageMultiplier -= damageMultiplierBonus;
        ctrl.hasKnockback = false;
        ctrl.TrimExcess();
    }
}
