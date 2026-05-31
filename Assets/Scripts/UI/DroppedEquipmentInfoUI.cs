using UnityEngine;

public class DroppedEquipmentInfoUI : MonoBehaviour
{
    [SerializeField] private ItemInfoUI itemInfoUI;
    [SerializeField] private ItemInfoUIFollowWorldTarget positionController;

    private Transform currentTarget;
    private bool isOpen;

    private void Awake()
    {
        if (itemInfoUI == null)
        {
            Debug.LogWarning("DroppedEquipmentInfoUI: itemInfoUI reference is missing.", this);
        }

        if (positionController == null)
        {
            Debug.LogWarning("DroppedEquipmentInfoUI: positionController reference is missing.", this);
        }
    }

    private void Update()
    {
        if (isOpen && currentTarget == null)
        {
            hideEquipmentInfo();
        }
    }

    public void openEquipmentInfo(EquipmentData equipment, Transform worldTarget)
    {
        if (itemInfoUI == null || worldTarget == null)
        {
            return;
        }

        currentTarget = worldTarget;
        isOpen = true;

        if (positionController != null)
        {
            positionController.setTarget(currentTarget);
        }

        itemInfoUI.openEquipmentInfo(equipment);
    }

    public void hideEquipmentInfo()
    {
        currentTarget = null;
        isOpen = false;

        if (positionController != null)
        {
            positionController.clearTarget();
        }

        if (itemInfoUI != null)
        {
            itemInfoUI.hideEquipmentInfo();
        }
    }
}
