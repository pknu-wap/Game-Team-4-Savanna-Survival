using UnityEngine;

// Structure Entry - 각 구조물 타입의 설정 데이터
[System.Serializable]
public class StructureEntry
{
    [SerializeField] public StructureType structureType;
    [SerializeField] public GameObject prefab;
    [SerializeField] public int minCount = 1;
    [SerializeField] public int maxCount = 3;
    [SerializeField] public float minDistance = 1.5f;
    [SerializeField] public int poolSize = 50;
}
