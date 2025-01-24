using UnityEngine;
using System.Collections;

public class AlienBehaviour : MonoBehaviour
{
    public Transform target;
    private int index;
    private Rigidbody rb;
    private Vector3 targetLocation;

    private float timer = 30f;  // Start with a 30-second timer
    private TextMesh timerText; //This whole text thing is gonna be replaced with a nice UI Later

    private static GameManager gameManager;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        targetLocation = new Vector3(target.position.x, transform.position.y,target.position.z);

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
            float distanceToTarget =  Vector3.Distance( transform.position,targetLocation);

            float stoppingDistance = 0.1f;
            if (distanceToTarget > stoppingDistance)
            {
                float speed = 3f;
                rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
            }
            else
            {
                rb.MovePosition(targetLocation);
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
        gameManager.AlienDeleted(index);
        Destroy(gameObject);
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
