using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputRelay : MonoBehaviour
{
    [SerializeField] private StatDetailPanel statDetailPanel;
    [SerializeField] private PlayerEquipmentPicker playerEquipmentPicker;

    public void OnStatAndInventory(InputValue value)
    {
        if (value.isPressed == false)
        {
            return;
        }

        statDetailPanel.toggleStatDetailPanel();
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed == false)
        {
            return;
        }

        playerEquipmentPicker.toggleInteract();
    } 
}