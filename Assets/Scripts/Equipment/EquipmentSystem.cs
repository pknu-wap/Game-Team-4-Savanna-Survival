using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    [Header("장비 데이터")]
    [SerializeField] private EquipmentData equipmentData;

    [SerializeField] private GameObject outlineObject;

    private void Awake()
    {
        setOutline(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerEquipmentPicker playerEquipmentPicker = other.GetComponent<PlayerEquipmentPicker>();

        if (playerEquipmentPicker == null)
        {
            return;
        }

        playerEquipmentPicker.addNearbyEquipment(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerEquipmentPicker playerEquipmentPicker = other.GetComponent<PlayerEquipmentPicker>();

        if (playerEquipmentPicker == null)
        {
            return;
        }

        playerEquipmentPicker.removeNearbyEquipment(this);
    }

    public void setOutline(bool isActive)
    {
        if (outlineObject == null)
        {
            return;
        }

        outlineObject.SetActive(isActive);
    }

    public void pickUp(PlayerStatManager playerStatManager)
    {
        if (playerStatManager == null)
        {
            return;
        }

        if (equipmentData == null)
        {
            Debug.LogError($"{gameObject.name}: EquipmentData 연결안됨");
            return;
        }

        playerStatManager.applyEquipmentList(equipmentData.equipmentStats);
        Destroy(gameObject);
    }
}