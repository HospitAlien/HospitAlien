using Oculus.Interaction.Locomotion;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    private GlobalVariableManager gvm;
    private PlayerLocomotor playerLocomotor;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        // Find the PlayerLocomotor script
        playerLocomotor = FindFirstObjectByType<PlayerLocomotor>();
        ChangeMovementStatus(gvm.IsMovementDisabled);
        // Listen the variable change event
        gvm.OnMovementChangedEvent += ChangeMovementStatus;
    }

    // Disable or enable the player's movement when the variable changes
    public void ChangeMovementStatus(bool IsMovementDisabled)
    {
        playerLocomotor.enabled = !IsMovementDisabled;
    }

    void OnDestroy()
    {
        // Unsubscribe from the event
        if (gvm != null)
        {
            gvm.OnMovementChangedEvent -= ChangeMovementStatus;
        }
    }
}