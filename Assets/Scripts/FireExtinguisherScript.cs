using UnityEngine;
using System.Linq; // Needed for .Any() or .FirstOrDefault()
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class FireExtinguisher : MonoBehaviour
{
    // Reference to the DistanceHandGrabInteractable on the fire extinguisher.
    // Assign this via the Inspector or let the script auto–find it.
    [SerializeField]
    private DistanceHandGrabInteractable _distanceGrabInteractable;

    private void Awake()
    {
        // If not assigned in the Inspector, try to find the interactable in our children.
        if (_distanceGrabInteractable == null)
        {
            _distanceGrabInteractable = GetComponentInChildren<DistanceHandGrabInteractable>();
            if (_distanceGrabInteractable == null)
            {
                Debug.LogError("DistanceHandGrabInteractable not found on children of " + gameObject.name);
            }
        }
    }

    private void Update()
    {
        // Check if the extinguisher is currently grabbed.
        // We do this by checking if the collection of selecting interactors is non–empty.
        if (_distanceGrabInteractable != null && _distanceGrabInteractable.SelectingInteractors.Any())
        {
            // Check for a trigger press on either controller.
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) ||
                OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                Debug.Log("Fire extinguisher trigger pressed while held!");
            }
        }
    }
}
