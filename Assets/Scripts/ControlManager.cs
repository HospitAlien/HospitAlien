using System.Collections.Generic;
using Oculus.Interaction.Locomotion;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    private GlobalVariableManager gvm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        // Listen the variable change event
        gvm.gameSettings.OnSettingChanged += ChangeMovementStatus;
    }

    public void ChangeMovementStatus(GameSettingsIO settings)
    {
    }

    // void Update()
    // {
    //     // If the player presses the "B" button on the right controller, change the movement status
    //     if (OVRInput.GetDown(OVRInput.Button.Two))
    //     {
    //         gvm.IsMovementDisabled = !gvm.IsMovementDisabled;
    //     }
    // }

    void OnDestroy()
    {
        // Unsubscribe the event
        gvm.gameSettings.OnSettingChanged -= ChangeMovementStatus;
    }
}