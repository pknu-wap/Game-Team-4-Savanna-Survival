using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Effects/PetCountIncreaseEffect")]
public class PetCountIncreaseEffect : PassiveEffect
{
public override void Apply(GameObject player)
    {
        var petCtrl = player.GetComponent<PetController>();
        if (petCtrl == null) return;
        petCtrl.maxPetCount++;
        player.GetComponent<PlayerSkillController>()?.TriggerAutoSkillGroup("wildness_baby_lion");
    }

    public override void Remove(GameObject player)
    {
        var petCtrl = player.GetComponent<PetController>();
        if (petCtrl != null) petCtrl.maxPetCount--;
    }
}
