using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Effects/PetCountIncreaseEffect")]
public class PetCountIncreaseEffect : PassiveEffect
{
    public override void Apply(GameObject player)
    {
        var petCtrl = player.GetComponent<PetController>();
        if (petCtrl != null) petCtrl.maxPetCount++;
    }

    public override void Remove(GameObject player)
    {
        var petCtrl = player.GetComponent<PetController>();
        if (petCtrl != null) petCtrl.maxPetCount--;
    }
}
