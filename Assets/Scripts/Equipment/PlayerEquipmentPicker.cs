using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEquipmentPicker : MonoBehaviour
{
    private readonly List<EquipmentPickup> nearbyEquipments = new List<EquipmentPickup>();

    [SerializeField] private EquipmentInventory equipmentInventory;
    private EquipmentPickup selectedEquipment;

    private void Update()
    {
        if (nearbyEquipments.Count <= 0)
        {
            return;
        }
        
        selectClosestEquipment();
    }

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
            selectedEquipment.setOutline(false);
            selectedEquipment = null;
        }

        nearbyEquipments.Remove(equipment);
    }

    private void selectClosestEquipment()
    {
        EquipmentPickup closestEquipment = getClosestEquipment();

        if (selectedEquipment == closestEquipment)
        {
            return;
        }

        if (selectedEquipment != null)
        {
            selectedEquipment.setOutline(false);
        }

        selectedEquipment = closestEquipment;

        if (selectedEquipment != null)
        {
            selectedEquipment.setOutline(true);
        }
    }

    private EquipmentPickup getClosestEquipment()
    {
        EquipmentPickup closestEquipment = null;
        float closestDistance = 100f;

        for (int i = nearbyEquipments.Count - 1; i >= 0; i--)
        {
            EquipmentPickup equipment = nearbyEquipments[i];

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
        nearbyEquipments.Remove(selectedEquipment);
        
        selectedEquipment.setOutline(false);
        selectedEquipment.pickUp(equipmentInventory);

        selectedEquipment = null;
    }
}