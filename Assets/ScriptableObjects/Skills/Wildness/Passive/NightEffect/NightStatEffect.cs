using UnityEngine;

[CreateAssetMenu(menuName = "SkillTree/Effects/NightStatEffect")]
public class NightStatEffect : PassiveEffect
{
    [SerializeField] private float damageBonus;
    [SerializeField] private float moveSpeedBonus;

    private PlayerStatManager statManager;
    private bool isApplied = false;

    public override void Apply(GameObject player)
    {
        statManager = player.GetComponent<PlayerStatManager>();
        if (statManager == null) return;
        if (statManager.StatCore == null) return;

        if (TimeManager.Instance == null) return;

        TimeManager.Instance.OnTimeStateChanged += OnTimeStateChanged;

        // 스킬을 배웠을 때 이미 밤이면 바로 적용
        if (TimeManager.Instance.IsDay == false)
        {
            ApplyNightBonus();
        }
    }

    public override void Remove(GameObject player)
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeStateChanged -= OnTimeStateChanged;
        }

        RemoveNightBonus();

        statManager = null;
    }

    private void OnTimeStateChanged(bool isDay)
    {
        if (isDay)
        {
            RemoveNightBonus();
        }
        else
        {
            ApplyNightBonus();
        }
    }

    private void ApplyNightBonus()
    {
        if (isApplied) return;
        if (statManager == null) return;
        if (statManager.StatCore == null) return;

        statManager.StatCore.addStat(StatType.DAMAGE, damageBonus);
        statManager.StatCore.addStat(StatType.MOVESPEED, moveSpeedBonus);

        isApplied = true;
    }

    private void RemoveNightBonus()
    {
        if (isApplied == false) return;
        if (statManager == null) return;
        if (statManager.StatCore == null) return;

        statManager.StatCore.addStat(StatType.DAMAGE, -damageBonus);
        statManager.StatCore.addStat(StatType.MOVESPEED, -moveSpeedBonus);

        isApplied = false;
    }
}