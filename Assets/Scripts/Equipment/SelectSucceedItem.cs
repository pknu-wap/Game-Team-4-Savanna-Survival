using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectSucceedItem : MonoBehaviour
{
    [SerializeField] private int maxSelectCount = 3;
    [SerializeField] private TempSucceedItem tempSucceedItem;

    private readonly List<EquipmentData> selectedEquipments = new List<EquipmentData>();

    public event Action onSelectionChanged;
    public event Action onMaxSelectionBlocked;

    public int SelectedCount => selectedEquipments.Count;
    public int MaxSelectCount => maxSelectCount;
    public IReadOnlyList<EquipmentData> SelectedEquipments => selectedEquipments;

    private void OnEnable()
    {
        clearSelection();
    }

    public bool isSelected(EquipmentData equipment)
    {
        return equipment != null && selectedEquipments.Contains(equipment);
    }

    public bool toggleSelect(EquipmentData equipment)
    {
        if (equipment == null)
        {
            return false;
        }

        if (selectedEquipments.Contains(equipment))
        {
            selectedEquipments.Remove(equipment);
            saveSelectedEquipments();
            onSelectionChanged?.Invoke();
            Debug.Log("선택 삭제");
            return true;
        }

        if (selectedEquipments.Count >= maxSelectCount)
        {
            onMaxSelectionBlocked?.Invoke();
            Debug.Log("초과됨");
            return false;
        }

        selectedEquipments.Add(equipment);
        saveSelectedEquipments();
        onSelectionChanged?.Invoke();
        Debug.Log("선택됨");
        return true;
    }

    public void clearSelection()
    {
        selectedEquipments.Clear();
        saveSelectedEquipments();
        onSelectionChanged?.Invoke();
    }

    private void saveSelectedEquipments()
    {
        if (tempSucceedItem != null)
        {
            tempSucceedItem.setItems(selectedEquipments);
        }
    }
}
