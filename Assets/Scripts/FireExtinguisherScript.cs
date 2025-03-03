using Oculus.Interaction.HandGrab;
using UnityEngine;

public class FireExtinguisherTrigger : MonoBehaviour, IHandGrabUseDelegate
{
    private float pressThreshold = 0.5f;

    public void BeginUse()
    {
        Debug.Log("Begin use");
    }

    public float ComputeUseStrength(float strength)
    {
        Debug.Log("Compute use strength, strength: " + strength);
        return strength;
    }

    public void EndUse()
    {
        Debug.Log("End use");
    }

    // private void Update()
    // {
    //     float leftTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
    //     float rightTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);

    //     bool isPressed = leftTrigger >= pressThreshold || rightTrigger >= pressThreshold;

    //     if (isPressed)
    //     {
    //         Debug.Log("Trigger is pressed!");
    //     }
    // }
}
