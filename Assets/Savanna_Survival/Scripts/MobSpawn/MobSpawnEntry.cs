using UnityEngine;

/// 개별 몹 소환 정보
[System.Serializable]
public class MobSpawnEntry
{
    public GameObject mobPrefab;
    public int weight = 1;
    public int minGroupSize = 1;
    public int maxGroupSize = 1;
}
