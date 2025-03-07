using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

struct Status
{
    public bool needsInjection;
    public bool needsExtinguishing;
    public bool needsAmputation;
    public bool hasShrapnel;
    public int shrapnelCount;

    public bool isHealthy()
    {
        return (!needsInjection && !needsExtinguishing && !hasShrapnel && !needsAmputation);
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
        else if (needsAmputation)
        {
            response += "My arm is ruined! \n";
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
    public Vector3 targetLocation;

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

    private float lastVoiceTime = -Mathf.Infinity;
    private float voiceCooldownTime = 3f;

    private Status status;
    private AlienVoice alienVoice;
    private int reward = 0;

    private Dictionary<Vector3, (Vector3, Quaternion)> beds
        = new Dictionary<Vector3, (Vector3, Quaternion)>();

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
        }

        if (random.NextDouble() < 0.4)
        {
            status.shrapnelCount = 4;
            reward += 100;
            status.hasShrapnel = true;
            initiateShrapnel();
        }

        if (random.NextDouble() < 0.4)
        {
            status.needsAmputation = true;
            initiateAmputation();
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

        beds[new Vector3(-2f, 2f, 2)] = (new Vector3(-2, 1, 2.575f), Quaternion.Euler(-90, 180, 0));
        beds[new Vector3(0f, 2f, 2f)] = (new Vector3(0, 1, 2.575f), Quaternion.Euler(-90, 180, 0));
        beds[new Vector3(2f, 2f, 2f)] = (new Vector3(2, 1, 2.575f), Quaternion.Euler(-90, 180, 0));
        beds[new Vector3(2f, 2f, 0f)] = (new Vector3(3.6f, 1, -1), Quaternion.Euler(-90, 180, 0));
        beds[new Vector3(2f, 2f, -2f)] = (new Vector3(2, 1, -2.5f), Quaternion.Euler(-90, 0, 0));
        beds[new Vector3(0f, 2f, -2f)] = (new Vector3(0, 1, -2.5f), Quaternion.Euler(-90, 0, 0));






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
                //Move the alien to bed and disable path-finding
                agent.Warp(beds[target.position].Item1);
                agent.enabled = false;
                transform.rotation = beds[target.position].Item2;
                target = null;
                isReady = true;
                //Reposition timer so it's not on the floor (it's rotated alongside the alien")
                Transform timerText = transform.Find("TimerText");
                if (timerText != null)
                {
                    timerText.localPosition = new Vector3(0f, 0.15f, 0.06f);
                    timerText.localRotation = Quaternion.Euler(-90f, 180f, 0f);
                }
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
        alienVoice.SayLine("You failed me!");

        gameManager.PatientDied(index);
        Destroy(gameObject);
    }

    void ChangeLeftHandMaterial()
    {
        // Finds the "left_hand" transform
        Transform leftHandTransform = transform.Find("hands/left_hand");
        if (leftHandTransform != null)
        {
            Renderer leftHandRenderer = leftHandTransform.GetComponent<Renderer>();
            if (leftHandRenderer != null)
            {
                // Change the material
                leftHandRenderer.material = Resources.Load<Material>("Amputate"); ;
            }
            else
            {
                Debug.LogError("Renderer component not found on left_hand");
            }
        }
        else
        {
            Debug.LogError("left_hand not found under hands");
        }
    }

    private void initiateAmputation()
    {
        // Create a new GameObject to detect axe hits
        GameObject amputationDetector = new GameObject("AmputationDetector");
        amputationDetector.transform.SetParent(transform);
        amputationDetector.transform.localPosition = Vector3.zero;
        amputationDetector.transform.localRotation = Quaternion.identity;
        amputationDetector.AddComponent<AmputationBehaviour>();
        ChangeLeftHandMaterial();
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

        if(status.shrapnelCount == 0){
            if (status.isHealthy())
            {
                Cure();
            }else{
                if (Time.time - lastVoiceTime >= voiceCooldownTime)
                {
                    alienVoice.SayLine("Thanks for removing the shrapnel");
                    lastVoiceTime = Time.time; // Update the time the voice line was last played
                }
            }
        }
    }

    public void shrapnelInserted()
    {
        status.shrapnelInserted();
    }





    // Function called when alien is cured
    public void Cure()
    {
        alienVoice.SayLine("Ah... Nuch better");

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
            GameObject newObject = Instantiate(coinParticlePrefab, spawnPosition, Quaternion.Euler(-90f, 0f, 0f));
            CoinBehaviour theCoins = newObject.GetComponent<CoinBehaviour>();
            theCoins.SetRate(12);

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
                status.needsExtinguishing = false;
                fireParticles.Stop();
                if (status.isHealthy())
                {
                    Cure();
                }else{
                    alienVoice.SayLine("the fire was put out");
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
                    alienVoice.SayLine("I really needed that injection!");  
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
