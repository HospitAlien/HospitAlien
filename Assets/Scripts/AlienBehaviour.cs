using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

struct Status{
    public bool needsInjection;
    public bool needsExtinguishing;
    public bool isHealthy()
    {
        return (!needsInjection && !needsExtinguishing);
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

    public ParticleSystem sweatParticles;

    public ParticleSystem fireParticles;

    private Status status;

    void InitiateStatus(){
        System.Random random = new System.Random();

        if(random.NextDouble() < 0.4){ 
            status.needsInjection = true;
            sweatParticles.Play();
        }
        if(random.NextDouble() < 0.4){
            status.needsExtinguishing = true;
            fireParticles.Play();
            Debug.Log("fire alien spawn"); //TODO there is a bug with water + fire aliens for tomorrow!!!
        }  
    }

    void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }



        agent = GetComponent<NavMeshAgent>();
        targetLocation = new Vector3(target.position.x, transform.position.y, target.position.z);
        agent.SetDestination(targetLocation);

        while (status.isHealthy())
        {
            InitiateStatus();
        }
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
        if (collider.CompareTag("Controller")) // Cure the alien if it collides with the controller
        {
            //veeraj's
        }
     
    }


    //Detect collision with fire extinguisher foam
    void OnParticleCollision(GameObject particle)
    {

        // Check if the particle colliding with the alien is from the fire extinguisher
        if (particle.CompareTag("Fire-Extinguisher"))
        {
            Debug.Log("FIRE Particle hit the alien!");

        }
        if(isReady)
        {
            if(particle.CompareTag("Fire-Extinguisher") && status.needsExtinguishing ){
                Debug.Log("CURED BY FOAM");
                status.needsExtinguishing = false;
                fireParticles.Stop();
                if(status.isHealthy()){
                    Cure();
                }
            }
        }
    }

    void Syrined(){

        if(isReady)
        {
            if(status.needsInjection ){
                status.needsInjection = false;
                if(status.isHealthy()){
                    Cure();
                }else{
                    sweatParticles.Stop();
                }
                
            }else{
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
