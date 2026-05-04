using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputRelay : MonoBehaviour
{
    [SerializeField] private StatDetailPanel statDetailPanel;

    public void OnStatDetailPanel(InputValue value)
    {
        if (value.isPressed == false)
        {
            return;
        }

        statDetailPanel.toggleStatDetailPanel();
    }
}