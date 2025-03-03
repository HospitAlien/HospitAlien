using UnityEngine;

public class FireExtinguisherTrigger : MonoBehaviour
{
    private float pressThreshold = 0.5f;


    private void Update()
    {
        float leftTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        float rightTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);

        bool isPressed = leftTrigger >= pressThreshold || rightTrigger >= pressThreshold;

        if (isPressed)
        {
            Debug.Log("Trigger is pressed!");
        }
    }
}
