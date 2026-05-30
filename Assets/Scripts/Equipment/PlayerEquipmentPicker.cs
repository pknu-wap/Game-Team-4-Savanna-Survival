using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEquipmentPicker : MonoBehaviour
{
    private readonly List<EquipmentPickup> nearbyEquipments = new List<EquipmentPickup>();

    [SerializeField] private EquipmentInventory equipmentInventory;
    [SerializeField] private DroppedEquipmentInfoUI droppedEquipmentInfoUI;
    [SerializeField] private float maxSelectionDistance = 3f;
    private EquipmentPickup selectedEquipment;

    private void Update()
    {
        removeInvalidNearbyEquipments();

        if (nearbyEquipments.Count <= 0)
        {
            clearSelectedEquipment();
            return;
        }
        
        selectClosestEquipment();
    }

    /*
    public void OnInteract(InputValue value)
    {
        if (value.isPressed == false)
        {
            return;
        }

        if (selectedEquipment == null)
        {
            return;
        }

        pickUpSelectedEquipment();
    } 
    */

    public void toggleInteract()
    {
        if (selectedEquipment == null)
        {
            return;
        }

        pickUpSelectedEquipment();
    }

    public void addNearbyEquipment(EquipmentPickup equipment)
    {
        if (nearbyEquipments.Contains(equipment))
        {
            return;
        }

        nearbyEquipments.Add(equipment);
    }

    public void removeNearbyEquipment(EquipmentPickup equipment)
    {
        if (equipment == selectedEquipment)
        {
            clearSelectedEquipment();
        }

        nearbyEquipments.Remove(equipment);
    }

    private void selectClosestEquipment()
    {
        EquipmentPickup closestEquipment = getClosestEquipment();

        if (closestEquipment == null)
        {
            clearSelectedEquipment();
            return;
        }

        if (selectedEquipment == closestEquipment)
        {
            return;
        }

        if (selectedEquipment != null)
        {
            clearSelectedEquipment();
        }

        selectedEquipment = closestEquipment;

        if (selectedEquipment != null)
        {
            selectedEquipment.setOutline(true);
            showSelectedEquipmentInfo();
        }
    }

    private EquipmentPickup getClosestEquipment()
    {
        EquipmentPickup closestEquipment = null;
        float closestDistance = maxSelectionDistance;

        for (int i = nearbyEquipments.Count - 1; i >= 0; i--)
        {
            EquipmentPickup equipment = nearbyEquipments[i];
            if (equipment == null || equipment.gameObject.activeInHierarchy == false)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, equipment.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEquipment = equipment;
            }
        }

        return closestEquipment;
    }

    private void pickUpSelectedEquipment()
    {
        EquipmentPickup equipmentToPickUp = selectedEquipment;
        nearbyEquipments.Remove(equipmentToPickUp);
        
        clearSelectedEquipment();
        equipmentToPickUp.pickUp(equipmentInventory);
    }

    private void removeInvalidNearbyEquipments()
    {
        for (int i = nearbyEquipments.Count - 1; i >= 0; --i)
        {
            if (nearbyEquipments[i] == null || nearbyEquipments[i].gameObject.activeInHierarchy == false)
            {
                if (nearbyEquipments[i] == selectedEquipment)
                {
                    clearSelectedEquipment();
                }

                nearbyEquipments.RemoveAt(i);
            }
        }
    }

    private void clearSelectedEquipment()
    {
        if (selectedEquipment != null)
        {
            selectedEquipment.setOutline(false);
        }

        hideSelectedEquipmentInfo();
        selectedEquipment = null;
    }

    private void showSelectedEquipmentInfo()
    {
        if (droppedEquipmentInfoUI == null || selectedEquipment == null || selectedEquipment.EquipmentData == null)
        {
            return;
        }

        droppedEquipmentInfoUI.openEquipmentInfo(selectedEquipment.EquipmentData, selectedEquipment.transform);
    }

    private void hideSelectedEquipmentInfo()
    {
        if (droppedEquipmentInfoUI != null)
        {
            droppedEquipmentInfoUI.hideEquipmentInfo();
        }
    }
}
