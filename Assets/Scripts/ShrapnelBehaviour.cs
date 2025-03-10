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

    void Update()
    {
        if (insideAlien)
        {
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;
        }
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
        Debug.Log("Object released");
        beingHeld = false;

        collider.isTrigger = false;

        if (!insideAlien)
        {
            destroyCoroutine = StartCoroutine(DestroyObjectAfterTime(5f));
            rb.isKinematic = false;
            Debug.Log("Kinematic false");
        }
        else
        {
            rb.isKinematic = true;
            Debug.Log("Kinematic true");
        }
    }

    private IEnumerator DestroyObjectAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    void OnTriggerExit(Collider collider)
    {

        if (!collider.CompareTag("Shrapnel") && beingHeld && insideAlien)
        {
            AlienBehaviour alien = collider.GetComponentInParent<AlienBehaviour>();
            if (alien != null)
            {
                Debug.Log("Trigger exit " + collider.name);
                alien.shrapnelRemoved();
                insideAlien = false;
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

                transform.SetParent(collider.transform.parent);


                if (destroyCoroutine != null)
                {
                    // Stop the coroutine if it's running
                    StopCoroutine(destroyCoroutine);
                    destroyCoroutine = null;
                    Debug.Log("Destroy coroutine stopped.");
                }

            }

        }
    }
}
