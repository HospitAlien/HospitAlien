using System;
using Oculus.Interaction;
using UnityEngine;
using System.Collections;

public class ShrapnelBehaviour : MonoBehaviour
{
    private bool insideAlien;
    private bool beingHeld;


    public GrabInteractable GrabInteractable;
    private Coroutine destroyCoroutine;

    private Rigidbody rb;
    private Collider collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        insideAlien = true;
        beingHeld = false;

        GrabInteractable.WhenSelectingInteractorAdded.Action += ObjectHeld;
        GrabInteractable.WhenSelectingInteractorRemoved.Action += ObjectReleased;
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    private void ObjectHeld(GrabInteractor interactor)
    {
        //when the object is held i want it to be isTrigger
        beingHeld = true;

        collider.isTrigger = true;

        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
            destroyCoroutine = null;
        }
    }

    private void ObjectReleased(GrabInteractor interactor)
    {
        beingHeld = false;

        collider.isTrigger = false;

        if (!insideAlien)
        {
            destroyCoroutine = StartCoroutine(DestroyObjectAfterTime(5f));
        }
    }

    private IEnumerator DestroyObjectAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    void OnTriggerExit(Collider collider)
    {
        if (!collider.CompareTag("Shrapnel") && beingHeld)
        {
            AlienBehaviour alien = collider.GetComponentInParent<AlienBehaviour>();
            if (alien != null)
            {
                alien.shrapnelRemoved();
                insideAlien = false;

                rb.isKinematic = false;
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag("Shrapnel") && !insideAlien)
        {
            AlienBehaviour alien = collider.GetComponentInParent<AlienBehaviour>();
            if (alien != null)
            {
                alien.shrapnelInserted();
                insideAlien = true;


                if (destroyCoroutine != null)
                {
                    // Stop the coroutine if it's running
                    StopCoroutine(destroyCoroutine);
                    destroyCoroutine = null;
                    Debug.Log("Destroy coroutine stopped.");

                    rb.isKinematic = true;
                }

            }

        }
    }
}
