using UnityEngine;

public class EquipmentPickup : MonoBehaviour
{
    [Header("장비 데이터")]
    // 장비 데이터 연결
    [SerializeField] private EquipmentData equipmentData;

    // 장비 선택시 하이라이트 연결
    [SerializeField] private GameObject outlineObject;

    private void Awake()
    {
        setOutline(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerEquipmentPicker playerEquipmentPicker = other.GetComponentInChildren<PlayerEquipmentPicker>();

        if (playerEquipmentPicker == null)
        {
            return;
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