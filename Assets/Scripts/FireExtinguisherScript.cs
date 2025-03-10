using Oculus.Interaction.HandGrab;
using UnityEngine;

public class FireExtinguisherTrigger : MonoBehaviour, IHandGrabUseDelegate
{
    private float pressThreshold = 0.5f;

    private ParticleSystem foam;
    private AudioSource spraySound;

    private void Awake()
    {
        foam = GetComponent<ParticleSystem>();
        spraySound = GetComponent<AudioSource>();
        
        if (foam == null)
        {
            Debug.LogError("No particle system found on fire extinguisher!");
        }
    }


    public void BeginUse()
    {
        foam.Play();
        Debug.Log("Begin use");
        spraySound.Play();
    }

    public float ComputeUseStrength(float strength)
    {
        Debug.Log("Compute use strength, strength: " + strength);
        return strength;
    }

    public void EndUse()
    {
        foam.Stop();
        Debug.Log("End use");
        spraySound.Stop();
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
