using UnityEngine;
using UnityEngine.XR;


public class FireExtinguisherTriggerDebug : MonoBehaviour
{
    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable; 

    private void Start()
    {
        // Get the VR controller devices for both hands
        leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Get the XRGrabInteractable component to check if the fire extinguisher is being held
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    private void Update()
    {
        // Check if the fire extinguisher is being grabbed
        if (grabInteractable.isSelected)
        {
            // Check if either hand is holding the fire extinguisher and the trigger button is pressed
            CheckForTriggerPress(leftHandDevice, "Left Hand Trigger Pressed!");
            CheckForTriggerPress(rightHandDevice, "Right Hand Trigger Pressed!");
        }
    }

    private void CheckForTriggerPress(InputDevice device, string logMessage)
    {
        if (device.isValid)
        {
            bool triggerPressed = false;
            if (device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed) && triggerPressed)
            {
                Debug.Log(logMessage);  // Log the message when trigger is pressed
            }
        }
    }
}