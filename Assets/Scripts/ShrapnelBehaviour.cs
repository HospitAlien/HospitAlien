using System;
using Oculus.Interaction;
using UnityEngine;
using System.Collections;

public class ShrapnelBehaviour : MonoBehaviour
{

    private bool insideAlien;
    public GrabInteractable GrabInteractable;
    private Coroutine destroyCoroutine;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        insideAlien = true;
        GrabInteractable.WhenSelectingInteractorRemoved.Action += ObjectReleased;
        rb = GetComponent<Rigidbody>();
    }

    private void ObjectReleased(GrabInteractor interactor)
    {
        if(!insideAlien){
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
        if (!collider.CompareTag("Shrapnel"))
        {
            AlienBehaviour alien = collider.GetComponentInParent<AlienBehaviour>();
            if (alien != null)
            {
                alien.shrapnelRemoved();
                insideAlien = false;

                rb.useGravity = true;
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


                if (destroyCoroutine != null)
                {
                    // Stop the coroutine if it's running
                    StopCoroutine(destroyCoroutine);
                    destroyCoroutine = null;
                    Debug.Log("Destroy coroutine stopped.");
                }
                rb.useGravity = false;

            }
            
        }
    }
}
