using System;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private int bossNight = 5;

    public static event Action<SkillTreeType> OnBossTypeSelected;

    private bool selected = false;

    private void Start()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeStateChanged += OnTimeStateChanged;
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeStateChanged -= OnTimeStateChanged;
    }

    private void OnTimeStateChanged(bool isDay)
    {
        if (isDay || selected) return;
        if (TimeManager.Instance.NightCount != bossNight - 1) return;

        SkillTreeType dominant = SkillManager.Instance != null
            ? SkillManager.Instance.GetDominantSkillTree()
            : SkillTreeType.None;

        // 더 적게 찍힌 쪽(dominant의 반대)이 보스로 결정.
        // 동률 시 랜덤.
        SkillTreeType bossType = dominant switch
        {
            SkillTreeType.Wildness => SkillTreeType.Fantasy,
            SkillTreeType.Fantasy  => SkillTreeType.Wildness,
            _                      => UnityEngine.Random.value > 0.5f ? SkillTreeType.Wildness : SkillTreeType.Fantasy
        };

        selected = true;
        OnBossTypeSelected?.Invoke(bossType);
        Debug.Log($"BossSpawner: 선택된 보스 타입 → {bossType}");
    }
}
