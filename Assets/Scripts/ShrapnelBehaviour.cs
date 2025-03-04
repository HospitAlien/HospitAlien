using System;
using Oculus.Interaction;
using UnityEngine;

public class ShrapnelBehaviour : MonoBehaviour
{

    private bool insideAlien;
    public GrabInteractable GrabInteractable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        insideAlien = true;
        GrabInteractable.WhenSelectingInteractorRemoved.Action += ObjectReleased;
    }

    private void ObjectReleased(GrabInteractor interactor)
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerExit(Collider collider)
    {
        if (!collider.CompareTag("Shrapnel"))
        {
            AlienBehaviour alien = collider.GetComponentInParent<AlienBehaviour>();
            if (alien != null)
            {
                alien.shrapnelRemoved();
                insideAlien = false;
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag("Shrapnel"))
        {
            AlienBehaviour alien = collider.GetComponentInParent<AlienBehaviour>();
            if (alien != null)
            {
                alien.shrapnelInserted();
                insideAlien = true;
            }
        }
    }
}
