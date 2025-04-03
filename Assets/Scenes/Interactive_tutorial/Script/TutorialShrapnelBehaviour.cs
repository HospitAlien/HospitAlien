using Oculus.Interaction;
using UnityEngine;
using System.Collections;

public class TutorialShrapnelBehaviour : MonoBehaviour
{
    public GrabInteractable GrabInteractable;
    public GameObject GrabableTip;
    public Outline outline;
    private bool outlineVisible;

    private Coroutine myCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        outlineVisible = false;
        outline.OutlineWidth = 0;
    }

    void Update()
    {
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

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Controller"))
        {
            GrabableTip.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Controller"))
        {
            GrabableTip.SetActive(true);
        }
    }
}
