using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Effects/MissileExplosionEffect")]
public class MissileExplosionEffect : PassiveEffect
{
    [SerializeField] float explosionRadius;

    public override void Apply(GameObject player)
    {
        var ctrl = player.GetComponent<MissileController>();
        if (ctrl == null) ctrl = player.AddComponent<MissileController>();
        ctrl.hasExplosion = true;
        ctrl.explosionRadius = explosionRadius;
    }

    public override void Remove(GameObject player)
    {
        var ctrl = player.GetComponent<MissileController>();
        if (ctrl == null) return;
        ctrl.hasExplosion = false;
        ctrl.explosionRadius = 0f;
    }
}
