using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

struct Status{
    public bool needsInjection;

    public bool isHealthy()
    {
        return (!needsInjection);
    }
}

public class AlienBehaviour : MonoBehaviour
{
    public Transform target;
    
    private int index;
    private Vector3 targetLocation;

    private float timer = 30f;  // Start with a 30-second timer
    private TextMesh timerText; //This whole text thing is gonna be replaced with a nice UI Later

    private static GameManager gameManager;
    private NavMeshAgent agent;

    private bool isReady = false; // Flag to check the patient have moved to right place
    private bool isCured = false; // Flag to check if the alien is cured

    private ParticleSystem sweatParticles;

    private Status status;

    void InitiateStatus(){
        System.Random random = new System.Random();

        if(random.NextDouble() < 0.5){
            status.needsInjection = true;
            sweatParticles.Play();
        }
 
    }

    void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }


        sweatParticles = GetComponent<ParticleSystem>();

        agent = GetComponent<NavMeshAgent>();
        targetLocation = new Vector3(target.position.x, transform.position.y, target.position.z);
        agent.SetDestination(targetLocation);


        InitiateStatus();
        InitiateTimer();
    }

    void InitiateTimer(){
        // Create a new TextMesh object for displaying the countdown
        GameObject timerGO = new GameObject("TimerText");
        timerGO.transform.SetParent(transform);
        timerGO.transform.localPosition = new Vector3(0, 0.2f, 0); // Position it above the alien's head

        timerText = timerGO.AddComponent<TextMesh>();
        timerText.fontSize = 100;
        timerText.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
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


            // Check if the agent has reached the destination
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                isReady = true;
            }
            else
            {
                agent.SetDestination(targetLocation);
                isReady = false;
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
        if (isReady && !status.needsInjection) // Only interact with the alien if it has reached the target location
        {
            if (collider.CompareTag("Controller")) // Cure the alien if it collides with the controller
            {
                Cure();
            }
        }
    }

    // Detect collision with syringe
    void OnCollisionEnter(Collision collision)
    {

        if(isReady)
        {
            if(collision.gameObject.CompareTag("Syringe")  && status.needsInjection ){
                status.needsInjection = false;
                sweatParticles.Stop();
                if(status.isHealthy()){
                    Cure();
                }
                collision.gameObject.tag = "Used-Syringe"; //We should probably move it to the syringe script
                

            }else if(collision.gameObject.CompareTag("Syringe")  && !status.needsInjection ){
                Debug.Log("KILLED BY SYRINGE");
                Delete();
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
