using System.Collections.Generic;
using UnityEngine;

public class EquipmentPickup : MonoBehaviour
{
    [Header("장비 데이터")]
    // 장비 데이터 연결
    [SerializeField] private EquipmentData equipmentData;

    // 장비 선택시 하이라이트 연결
    [SerializeField] private GameObject outlineObject;

    private readonly List<PlayerEquipmentPicker> nearbyPickers = new List<PlayerEquipmentPicker>();

    public EquipmentData EquipmentData => equipmentData;

    private void Awake()
    {
        // setOutline(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerEquipmentPicker playerEquipmentPicker = other.GetComponentInChildren<PlayerEquipmentPicker>();

        if (playerEquipmentPicker == null)
        {
            return;
        }

        if (nearbyPickers.Contains(playerEquipmentPicker) == false)
        {
            nearbyPickers.Add(playerEquipmentPicker);
        }

        playerEquipmentPicker.addNearbyEquipment(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerEquipmentPicker playerEquipmentPicker = other.GetComponentInChildren<PlayerEquipmentPicker>();

        if (playerEquipmentPicker == null)
        {
            return;
        }

        playerEquipmentPicker.removeNearbyEquipment(this);
        nearbyPickers.Remove(playerEquipmentPicker);
    }

    private void OnDisable()
    {
        for (int i = nearbyPickers.Count - 1; i >= 0; --i)
        {
            if (nearbyPickers[i] != null)
            {
                nearbyPickers[i].removeNearbyEquipment(this);
            }
        }

        nearbyPickers.Clear();
    }

    public void setOutline(bool isActive)
    {
        outlineObject.SetActive(isActive);
    }

    public void pickUp(EquipmentInventory equipmentInventory)
    {
        bool isAdded = equipmentInventory.addInventoryEquipment(equipmentData);

        if (!isAdded) return;
        // playerStatManager.applyEquipmentList(equipmentData.equipmentStats);
        Destroy(gameObject);
    }
}
