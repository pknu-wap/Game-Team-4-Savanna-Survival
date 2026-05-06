using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    private PlayerStatCore statCore;
    [SerializeField] private PlayerStatManager playerStatManager;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider hungerBar;
    [SerializeField] private Slider expBar;

    private void Start()
    {
        statCore = playerStatManager.StatCore;
        statCore.onStatRegistered += onStatRegistered;

        healthBar.value = statCore.getStat(StatType.HEALTH).rawValue / statCore.getStat(StatType.MAX_HEALTH).rawValue;
        hungerBar.value = statCore.getStat(StatType.HUNGER).rawValue / statCore.getStat(StatType.MAX_HUNGER).rawValue;
        expBar.value = statCore.getStat(StatType.EXP).rawValue / statCore.getStat(StatType.MAX_EXP).rawValue;
    }

    private void onStatRegistered(StatType statType, float value)
    {
        switch (statType)
        {
            case StatType.HEALTH:
            case StatType.MAX_HEALTH:
                handleBar(statCore.getStat(StatType.HEALTH).rawValue, statCore.getStat(StatType.MAX_HEALTH).rawValue, healthBar);
                break;

            case StatType.EXP:
            case StatType.MAX_EXP:
            case StatType.LEVEL:
                handleBar(statCore.getStat(StatType.EXP).rawValue, statCore.getStat(StatType.MAX_EXP).rawValue, expBar);
                break;

            case StatType.HUNGER:
            case StatType.MAX_HUNGER:
                handleBar(statCore.getStat(StatType.HUNGER).rawValue, statCore.getStat(StatType.MAX_HUNGER).rawValue, hungerBar);
                break;
        }
    }

    private void handleBar(float current, float max, Slider bar)
    {
        float handleValue = current / max;
        bar.value = Mathf.Clamp01(handleValue);
    }
    private void OnDestroy() //이벤트 해제용
    {
        if (statCore != null)
        {
            statCore.onStatRegistered -= onStatRegistered;
        }
    }
}
