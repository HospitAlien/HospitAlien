using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

struct Status
{
    public bool needsInjection;
    public bool needsExtinguishing;
    public bool hasShrapnel;
    public int shrapnelCount;
    public int curSize;
    public int numberOfBadEyes;


    public bool isHealthy()
    {
        return (!needsInjection && !needsExtinguishing && !hasShrapnel && curSize == 0 && numberOfBadEyes == 0);
    }

    public string getIllness()
    {
        string response = string.Empty;
        if (needsExtinguishing)
        {
            response += "I'm burning, grab the fire extinguisher and put me out.\n";
        }
        else if (needsInjection)
        {
            response += "I need an injection.\n";
        }
        else if (hasShrapnel)
        {
            response += "There is shrapnel inside of me, grab it and pull it out.\n";
        }
        else if (curSize == -1)
        {
            response += "I am really small get a growth pill.\n";
        }
        else if (curSize == 1)
        {
            response += "I am massive get me a shrink pill.\n";
        }
        else if(numberOfBadEyes != 0)
        {
            response += "I have a really itchy eye, get me an eyedrop.\n";
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

    public void applyEyeDrop(){
        numberOfBadEyes = numberOfBadEyes -1;
    }

}

public class AlienBehaviour : MonoBehaviour
{
    public Transform target;

    private int index;
    public Vector3 targetLocation;

    private float timer = 120;  // Start with a 120-second timer
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

    public AudioSource growthSFX;
    public AudioSource shrinkSFX;

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
            reward += 50;
        }

        if (random.NextDouble() < 0.4)
        {
            status.shrapnelCount = 4;
            reward += 150;
            status.hasShrapnel = true;
            initiateShrapnel();
        }

        if (random.NextDouble() < 0.4)
        {
            reward += 100;
            if (random.NextDouble() < 0.5)
            { //shrink
                status.curSize = -1;
                transform.localScale /= 2f;
            }
            else
            { //enlargement
                status.curSize = 1;
                transform.localScale *= 1.4f;
            }
        }

        if(random.NextDouble() < 0.4){
            status.numberOfBadEyes = 1;
            int eyeNumber = random.Next(1,4);
            Debug.Log(eyeNumber);
            EyeScript eye = transform.Find("eye"+Convert.ToString(eyeNumber)).gameObject.GetComponent<EyeScript>();
            eye.activate(this);
        }


    }

    void Start()
    {
        alienVoice = GetComponent<AlienVoice>();

        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
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

    public void applyEyedrop(){
        status.applyEyeDrop();
        if(status.isHealthy()){
            Cure();
        }
    }


    private void initiateShrapnel()
    {
        int count = 0;
        while (count < 4)
        {
            // Use Unity's Random class for generating random numbers
            float offsetX = UnityEngine.Random.Range(-0.3f, 0.3f);
            float offsetY = UnityEngine.Random.Range(0f, 0.5f);
            float offsetZ = 0.1f;  // Depth offset (can be used to spawn metal further into the body)

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

        if (status.shrapnelCount == 0)
        {
            if (status.isHealthy())
            {
                Cure();
            }
            else
            {
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
        alienVoice.SayLine("Ah... Much better");

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
                }
                else
                {
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

    void EnlargementPilled()
    {

        if (status.curSize == -1)
        {
            status.curSize += 1;
            growthSFX.Play();
            StartCoroutine(ScaleOverTime(2f, growthSFX.clip.length));
        }
        else if (status.curSize == 0)
        {
            status.curSize += 1;
            growthSFX.Play();
            StartCoroutine(ScaleOverTime(1.4f, growthSFX.clip.length));
        }

        if (status.curSize == 0)
        {
            if (status.isHealthy())
            {
                Cure();
            }
        }
    }

    void ShrinkPilled()
    {

        if (status.curSize == 1)
        {
            status.curSize -= 1;
            shrinkSFX.Play();
            StartCoroutine(ScaleOverTime((1 / (1.4f)), shrinkSFX.clip.length));
        }
        else if (status.curSize == 0)
        {
            status.curSize -= 1;
            shrinkSFX.Play();
            StartCoroutine(ScaleOverTime(0.5f, shrinkSFX.clip.length));
        }

        if (status.curSize == 0)
        {
            if (status.isHealthy())
            {
                Cure();
            }
        }
    }


    IEnumerator ScaleOverTime(float targetMultiplier, float duration)
    {
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = initialScale * targetMultiplier;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final scale is exactly the target
        transform.localScale = targetScale;
    }






    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetIndex(int newIndex)
    {
        index = newIndex;
    }


    // Functions to pass info to game manager to pass to music manager

    public float GetRemainingTime()
    {
        return timer;
    }

    public int GetInjuryCount()
    {
        int injuries = 0;
        if (status.needsInjection) injuries++;
        if (status.needsExtinguishing) injuries++;
        if (status.hasShrapnel) injuries++;
        return injuries;
    }
}