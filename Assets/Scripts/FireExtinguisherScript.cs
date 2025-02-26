using UnityEngine;
using Oculus.Interaction.HandGrab; // For DistanceHandGrabInteractable
using Oculus.Interaction.Input;    // For OVRInput
using System.Linq;                 // For Any()

public class FireExtinguisherOVR : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the DistanceHandGrabInteractable that manages grabbing this extinguisher.")]
    [SerializeField]
    private DistanceHandGrabInteractable _distanceHandGrab;

    [Tooltip("Particle system simulating the fire extinguisher spray.")]
    [SerializeField]
    private ParticleSystem _extinguisherParticle;

    [Header("Trigger Thresholds")]
    [Tooltip("Above this trigger value (0 to 1) the extinguisher spray starts.")]
    [Range(0f, 1f)]
    [SerializeField]
    private float _fireThreshold = 0.9f;

    [Tooltip("Below this trigger value (0 to 1) the extinguisher spray stops.")]
    [Range(0f, 1f)]
    [SerializeField]
    private float _releaseThreshold = 0.2f;

    // Internal state: whether the spray is currently active.
    private bool _isFiring = false;

    private void Update()
    {
        // Check if the extinguisher is currently grabbed
        bool isGrabbed = _distanceHandGrab != null && _distanceHandGrab.SelectingInteractors.Any();

        if (!isGrabbed)
        {
            // If not grabbed, ensure any active spray is stopped.
            if (_isFiring)
            {
                Debug.Log("this shouldn't appear");
            }
            return;
        }

        // Read the trigger value using OVRInput.
        // We check both the primary (left) and secondary (right) index triggers and use the larger value.
        float leftTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        float rightTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        float triggerValue = Mathf.Max(leftTrigger, rightTrigger);

        // Start spraying if trigger exceeds the fire threshold.
        if (triggerValue >= _fireThreshold && !_isFiring)
        {
            Debug.Log("trigger pressed");
        }
    }

    
}
