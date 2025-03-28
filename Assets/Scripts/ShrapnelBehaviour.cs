using Oculus.Interaction;
using UnityEngine;
using System.Collections;

public class ShrapnelBehaviour : MonoBehaviour
{
    private bool insideAlien;
    private bool beingHeld;


    public GrabInteractable GrabInteractable;
    public GameObject GrabableTip;
    private Coroutine destroyCoroutine;

    private Rigidbody rb;
    private Collider _collider;

    public Outline outline;
    private bool outlineVisible;

    private Coroutine myCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        outlineVisible = false;
        outline.OutlineWidth = 0;
        insideAlien = true;
        beingHeld = false;

        GrabInteractable.WhenSelectingInteractorAdded.Action += ObjectHeld;
        GrabInteractable.WhenSelectingInteractorRemoved.Action += ObjectReleased;
        rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
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

        Camera mainCamera = Camera.main;
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);

        if (distance > 2)
        {
            if (outlineVisible)
            {
                if (myCoroutine != null)
                {
                    StopCoroutine(myCoroutine);
                }
                outline.OutlineWidth = 0;
                outlineVisible = false;
            }

        }
        else if (distance <= 2)
        {
            if (!outlineVisible)
            {
                myCoroutine = StartCoroutine(Pulse());
                outlineVisible = true;
            }
        }

    }

    IEnumerator Pulse()
    {
        float transitionTime = 1f;
        float targetValue = 4f;
        float startValue = outline.OutlineWidth;
        float elapsedTime = 0f;

        while (true)
        {
            while (elapsedTime < transitionTime)
            {
                outline.OutlineWidth = Mathf.Lerp(startValue, targetValue, elapsedTime / transitionTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            outline.OutlineWidth = targetValue;

            targetValue = startValue;
            startValue = outline.OutlineWidth;


            elapsedTime = 0f;
        }
    }


    private void ObjectHeld(GrabInteractor interactor)
    {
        //when the object is held i want it to be isTrigger
        beingHeld = true;

        _collider.isTrigger = true;

        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
            destroyCoroutine = null;
        }
    }

    private void ObjectReleased(GrabInteractor interactor)
    {
        beingHeld = false;

        _collider.isTrigger = false;

        if (!insideAlien)
        {
            destroyCoroutine = StartCoroutine(DestroyObjectAfterTime(5f));
            rb.isKinematic = false;
        }
        else
        {
            rb.isKinematic = true;
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
            Alien alien = collider.GetComponentInParent<Alien>();
            if (alien != null)
            {
                alien.shrapnelRemoved();
                insideAlien = false;
            }
        }
        if (collider.CompareTag("Controller"))
        {
            GrabableTip.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag("Shrapnel") && !insideAlien)
        {
            Alien alien = collider.GetComponentInParent<Alien>();
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
                }

            }
        }
        if (collider.CompareTag("Controller"))
        {
            GrabableTip.SetActive(true);
        }
    }
}
