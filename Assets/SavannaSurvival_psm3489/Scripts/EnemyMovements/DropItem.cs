using UnityEngine;

[System.Serializable]
public class DropItem
{
    public GameObject itemPrefab;

    [Range(0f, 1f)]
    public float dropChance = 0.5f;

    public int minAmount = 1;
    public int maxAmount = 1;
}