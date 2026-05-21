using UnityEngine;

public class DamageBoostEffect : PassiveEffect
{
    public override void Apply(GameObject player)
    {
        Debug.Log("Damage boost applied!");
    }

    public override void Remove(GameObject player)
    {
        Debug.Log("Damage boost removed!");
    }
}
