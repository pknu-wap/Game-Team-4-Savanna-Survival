using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Effects/PetDamageIncreaseEffect")]
public class PetDamageIncreaseEffect : PassiveEffect
{
    public float bonusMultiplier = 0.15f;

    public override void Apply(GameObject player)
    {
        var petCtrl = player.GetComponent<PetController>();
        if (petCtrl != null) petCtrl.petDamageMultiplier += bonusMultiplier;
    }

    public override void Remove(GameObject player)
    {
        var petCtrl = player.GetComponent<PetController>();
        if (petCtrl != null) petCtrl.petDamageMultiplier -= bonusMultiplier;
    }
}
