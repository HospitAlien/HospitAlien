using UnityEngine;
using System.Collections;
using System;

public class AlienBehaviour : MonoBehaviour
{
    public Transform target;
    private int index;
    private Rigidbody rb;
    private Vector3 targetLocation;

    private float timer = 30f;  // Start with a 30-second timer
    private TextMesh timerText; //This whole text thing is gonna be replaced with a nice UI Later

    private static GameManager gameManager;

    private bool isReady = false; // Flag to check the patient have moved to right place
    private bool isCured = false; // Flag to check if the alien is cured



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        targetLocation = new Vector3(target.position.x, transform.position.y, target.position.z);

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }


        // Create a new TextMesh object for displaying the countdown
        GameObject timerGO = new GameObject("TimerText");
        timerGO.transform.SetParent(transform);
        timerGO.transform.localPosition = new Vector3(0, 2, 0); // Position it above the alien's head

        timerText = timerGO.AddComponent<TextMesh>();
        timerText.fontSize = 100;
        timerText.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        timerText.color = Color.black;
        timerText.alignment = TextAlignment.Center;
        timerText.anchor = TextAnchor.MiddleCenter;

        // Start the countdown coroutine
        StartCoroutine(CountdownTimer());

    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 direction = (targetLocation - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, targetLocation);

            float stoppingDistance = 0.1f;
            if (distanceToTarget > stoppingDistance)
            {
                float speed = 3f;
                isReady = false;
                rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
            }
            else
            {
                rb.MovePosition(targetLocation);
                isReady = true;
            }
        }
    }

    private IEnumerator CountdownTimer()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime; // Decrease the timer by time passed
            timerText.text = Mathf.Ceil(timer).ToString(); // Update the text to show the remaining time
            yield return null; // Wait for the next frame
        }

        // After the countdown, delete the object
        Delete();
    }

    // Function to delete the alien object
    void Delete()
    {
        gameManager.PatientDied(index);
        Destroy(gameObject);
    }

    // Function called when alien is cured
    public void Cure()
    {
        if (isCured)
        {
            return; // Prevent curing the same alien multiple times
        }
        isCured = true;
        gameManager.PatientCured(index);
        Destroy(gameObject);
    }

    // Detect collision with the player's controller
    void OnTriggerEnter(Collider collider)
    {
        if (isReady) // Only interact with the alien if it has reached the target location
        {
            if (collider.CompareTag("Controller")) // Cure the alien if it collides with the controller
            {
                Cure();
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetIndex(int newIndex)
    {
        index = newIndex;
    }
}
