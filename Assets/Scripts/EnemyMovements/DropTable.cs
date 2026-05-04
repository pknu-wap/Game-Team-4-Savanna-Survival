using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Drop/Drop Table")]
public class DropTable : ScriptableObject
{
    [Header("확정 드랍")]
    public List<DropItem> guaranteedDrops;

    [Header("확률 드랍")]
    public List<DropItem> randomDrops;

    public void Drop(Vector2 position)
    {
        foreach (var drop in guaranteedDrops)
        {
            SpawnDrop(drop, position);
        }

        foreach (var drop in randomDrops)
        {
            if (Random.value <= drop.dropChance)
            {
                SpawnDrop(drop, position);
            }
        }
    }

    private void SpawnDrop(DropItem drop, Vector2 position)
    {
        int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);

        for (int i = 0; i < amount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 0.5f;

            Instantiate(
                drop.itemPrefab,
                position + offset,
                Quaternion.identity
            );
        }
    }
}