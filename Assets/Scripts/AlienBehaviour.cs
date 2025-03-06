using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;
using System.Linq;

struct Status
{
    public bool needsInjection;
    public bool needsExtinguishing;
    public bool hasShrapnel;
    public int shrapnelCount;

    public bool isHealthy()
    {
        return (!needsInjection && !needsExtinguishing && !hasShrapnel);
    }

    public string getIllness()
    {
        string response = string.Empty;
        if (needsExtinguishing)
        {
            response += "I'm burning.\n";
        }
        else if (needsInjection)
        {
            response += "I need a jab.\n";
        }

        return response;
    }

    public void shrapnelRemoved()
    {
        shrapnelCount = shrapnelCount - 1;
        if (shrapnelCount == 0)
        {
            hasShrapnel = false;
        }
    }

    public void shrapnelInserted()
    {
        shrapnelCount = shrapnelCount + 1;
        hasShrapnel = true;
    }

}

public class AlienBehaviour : MonoBehaviour
{
    public Transform target;

    private int index;
    private Vector3 targetLocation;

    private float timer = 60;  // Start with a 60-second timer
    private TextMesh timerText; //This whole text thing is gonna be replaced with a nice UI Later

    private static GameManager gameManager;
    private NavMeshAgent agent;

    private bool isReady = false; // Flag to check the patient have moved to right place
    private bool isCured = false; // Flag to check if the alien is cured

    public ParticleSystem sweatParticles;
    public ParticleSystem fireParticles;
    public GameObject shrapnel;
    public Transform bodyTransform; //used to find the body 
    public GameObject coinParticlePrefab;

    private Status status;
    private AlienVoice alienVoice;
    private int reward = 0;

    public string getVoiceLine()
    {
        return status.getIllness();
    }

    //TODO: I think its better to roll a random number to decide how many troubles the alien has then select from a list with weighted probabilities
    void InitiateStatus()
    {
        System.Random random = new System.Random();

        if (random.NextDouble() < 0.4)
        {
            status.needsInjection = true;
            sweatParticles.Play();
            reward += 100;
        }
        if (random.NextDouble() < 0.4)
        {
            status.needsExtinguishing = true;
            fireParticles.Play();
            reward += 100;
            Debug.Log("fire alien spawn"); //TODO there is a bug with water + fire aliens for tomorrow!!!
        }

        if (random.NextDouble() < 0.4)
        {
            status.shrapnelCount = 4;
            reward += 100;
            status.hasShrapnel = true;
            initiateShrapnel();
        }


    }

    void Start()
    {
        alienVoice = GetComponent<AlienVoice>();

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

    void InitiateTimer()
    {
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


    private void initiateShrapnel()
    {
        int count = 0;
        while (count < 4)
        {
            // Use Unity's Random class for generating random numbers
            float offsetX = UnityEngine.Random.Range(-0.3f, 0.3f);
            float offsetY = UnityEngine.Random.Range(0f, 0.5f);
            float offsetZ = 0.1f;  // Depth offset (use if you want to spawn swords further into the body)

            // Spawn the new shrapnel at the calculated position
            Vector3 newPosition = bodyTransform.position + new Vector3(offsetX, offsetY, offsetZ);

            float randomXRotation = UnityEngine.Random.Range(-30f, 30f);
            float randomZRotation = UnityEngine.Random.Range(0f, 360f);

            // Construct a new rotation with random x and z rotation values, keeping the y rotation the same as bodyTransform
            Quaternion randomRotation = Quaternion.Euler(randomXRotation, bodyTransform.rotation.eulerAngles.y, randomZRotation);



            GameObject newShrapnel = Instantiate(shrapnel, newPosition, randomRotation);
            newShrapnel.transform.SetParent(transform); // Set the shrapnel as a child of the player

            count++;
        }
    }

    public void shrapnelRemoved()
    {
        status.shrapnelRemoved();
        Debug.Log("Removed " + status.shrapnelCount);
        if (status.isHealthy())
        {
            Cure();
        }
    }

    public void shrapnelInserted()
    {
        Debug.Log("INSERTED");
        status.shrapnelInserted();
    }





    // Function called when alien is cured
    public void Cure()
    {
        if (isCured)
        {
            return; // Prevent curing the same alien multiple times
        }
        isCured = true;
        gameManager.PatientCured(index, reward);

        Debug.Log("Spawn coins");

        if (coinParticlePrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0f, 0.5f, 0f);
            // Instantiate the particle system at the current position and with the current rotation
            Instantiate(coinParticlePrefab, spawnPosition, Quaternion.Euler(-90f, 0f, 0f));
            Debug.Log("Coins spawned");
        }

        Destroy(gameObject);
    }

    // Detect collision with the player's controller
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("AlienTranslator")) // Cure the alien if it collides with the controller
        {
            //Voice chat function
            alienVoice.ActivateListening();
        }

    }


    //Detect collision with fire extinguisher foam
    void OnParticleCollision(GameObject particle)
    {

        // Check if the particle colliding with the alien is from the fire extinguisher
        // if (particle.CompareTag("Fire-Extinguisher"))
        // {
        //     Debug.Log("FIRE Particle hit the alien!");

        // }
        if (isReady)
        {
            if (particle.CompareTag("Fire-Extinguisher") && status.needsExtinguishing)
            {
                Debug.Log("CURED BY FOAM");
                status.needsExtinguishing = false;
                fireParticles.Stop();
                if (status.isHealthy())
                {
                    Cure();
                }
            }
        }
    }

    void Syrined()
    {

        if (isReady)
        {
            if (status.needsInjection)
            {
                status.needsInjection = false;
                if (status.isHealthy())
                {
                    Cure();
                }
                else
                {
                    sweatParticles.Stop();
                }

            }
            else
            {
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
