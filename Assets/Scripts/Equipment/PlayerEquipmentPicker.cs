using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEquipmentPicker : MonoBehaviour
{
    private readonly List<EquipmentPickup> nearbyEquipments = new List<EquipmentPickup>();

    private PlayerStatManager playerStatManager;
    private EquipmentPickup selectedEquipment;

    private void Awake()
    {
        playerStatManager = GetComponentInParent<PlayerStatManager>();

        if (playerStatManager == null)
        {
            Debug.LogError("PlayerEquipmentPicker: PlayerStatManager를 못찾음");
        }
    }

    private void Update()
    {
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
        if (equipment == null)
        {
            return;
        }

        if (nearbyEquipments.Contains(equipment))
        {
            return;
        }

        nearbyEquipments.Add(equipment);
    }

    public void removeNearbyEquipment(EquipmentPickup equipment)
    {
        if (equipment == null)
        {
            return;
        }

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
        float closestDistanceSqr = float.MaxValue;

        for (int i = nearbyEquipments.Count - 1; i >= 0; i--)
        {
            EquipmentPickup equipment = nearbyEquipments[i];

            if (equipment == null)
            {
                nearbyEquipments.RemoveAt(i);
                continue;
            }

            float distanceSqr = ((Vector2)transform.position - (Vector2)equipment.transform.position).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestEquipment = equipment;
            }
        }

        return closestEquipment;
    }

    private void pickUpSelectedEquipment()
    {
        EquipmentPickup equipmentToPickUp = selectedEquipment;

        if (equipmentToPickUp == null)
        {
            return;
        }

        nearbyEquipments.Remove(equipmentToPickUp);
        selectedEquipment = null;

        equipmentToPickUp.setOutline(false);
        equipmentToPickUp.pickUp(playerStatManager);
    }
}