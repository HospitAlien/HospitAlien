using System.Collections.Generic;
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
        if (playerLocomotor != null)
        {
            if (IsMovementDisabled)
            {
                playerLocomotor.enabled = false;
            }
            else
            {
                var eventQueue = typeof(PlayerLocomotor).GetField("_deferredEvent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (eventQueue != null)
                {
                    var queue = (Queue<LocomotionEvent>)eventQueue.GetValue(playerLocomotor);
                    queue.Clear();
                }
                playerLocomotor.enabled = true;
            }
        }
    }

    void Update()
    {
        // If the player presses the "B" button on the right controller, change the movement status
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            gvm.IsMovementDisabled = !gvm.IsMovementDisabled;
        }
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