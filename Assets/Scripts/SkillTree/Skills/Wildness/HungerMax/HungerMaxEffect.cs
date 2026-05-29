using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Effects/HungerMaxEffect")]
public class HungerMaxEffect : PassiveEffect
{
    public float damageBonus;
    public float speedBonus;
    public float buffDuration;

    public override void Apply(GameObject player)
    {
        var buff = player.GetComponent<HungerMaxBuff>();
        if (buff == null)
            buff = player.AddComponent<HungerMaxBuff>();

        buff.damageBonus = damageBonus;
        buff.speedBonus = speedBonus;
        buff.buffDuration = buffDuration;
    }

    public override void Remove(GameObject player)
    {
        var buff = player.GetComponent<HungerMaxBuff>();
        if (buff != null)
            Destroy(buff);
    }
}
