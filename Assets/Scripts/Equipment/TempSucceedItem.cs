using System.Collections.Generic;
using UnityEngine;

public class TempSucceedItem : MonoBehaviour
{
    [SerializeField] private List<EquipmentData> succeedItems = new List<EquipmentData>();

    public IReadOnlyList<EquipmentData> SucceedItems => succeedItems;

    public void setItems(IReadOnlyList<EquipmentData> equipments)
    {
        succeedItems.Clear();

        if (equipments == null)
        {
            return;
        }

        for (int i = 0; i < equipments.Count; ++i)
        {
            if (equipments[i] != null)
            {
                succeedItems.Add(equipments[i]);
            }
        }
    }

    public void clearItems()
    {
        succeedItems.Clear();
    }
}
