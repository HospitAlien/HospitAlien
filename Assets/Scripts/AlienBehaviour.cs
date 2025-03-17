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
    public bool needsLeftHand;
    public int shrapnelCount;
    public int curSize;


    public bool isHealthy()
    {
        return (!needsInjection && !needsExtinguishing && !hasShrapnel && curSize == 0 && !needsAmputation && !needsLeftHand);
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
        else if (hasShrapnel)
        {
            response += "There is shrapnel inside of me.\n";
        }
        else if (curSize == -1)
        {
            response += "I am really small.\n";
        }
        else if (curSize == 1)
        {
            response += "I am massive.\n";
        }
        else if (needsAmputation)
        {
            response += "My arm is ruined! \n";
        }
        else if (needsLeftHand)
        {
            response += "I have no hand! \n";
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

public struct Amputations
{
    public bool[] limbs;
    
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

    public AudioSource growthSFX;
    public AudioSource shrinkSFX;

    private float lastVoiceTime = -Mathf.Infinity;
    private float voiceCooldownTime = 3f;

    private Status status;
    public Amputations amputations;
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

        if (random.NextDouble() < 1)
        {
            status.needsAmputation = true;
            amputations.limbs = new bool[] { true, false, false, false };
            initiateAmputation();
        }
        if(random.NextDouble() < 0.4){

            if(random.NextDouble() < 0.5){ //shrink
                status.curSize = -1;
                transform.localScale /= 2f;
            }else{ //enlargement
                status.curSize = 1;
                transform.localScale *= 1.4f;
            }

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

    void ChangeLimbMaterial(Transform limbTransform)
    {
        // Finds the "left_hand" transform
        if (limbTransform != null)
        {
            Renderer limbRenderer = limbTransform.GetComponent<Renderer>();
            if (limbRenderer != null)
            {
                // Change the material
                limbRenderer.material = Resources.Load<Material>("Amputate"); ;
            }
            else
            {
                Debug.LogError("Renderer component not found on limb");
            }
        }
        else
        {
            Debug.LogError("limb not found");
        }
    }


    //Add a child to hold colliders to detect axe hits
    private void initiateAmputation()
    {

        System.Random random = new System.Random();
        for (int i = 1; i < 4; i++)
        {
            if (random.NextDouble() < 0.4)
            {
                amputations.limbs[i] = true;
            }
        }
              

        // Create a new GameObject to detect axe hits
        GameObject amputationDetector = new GameObject("AmputationDetector");
        amputationDetector.transform.SetParent(transform);
        amputationDetector.transform.localPosition = Vector3.zero;
        amputationDetector.transform.localRotation = Quaternion.identity;
        amputationDetector.AddComponent<AmputationBehaviour>();

        for (int i = 0; i < 4; i++)
        {
            if (amputations.limbs[i])
            {
                switch (i)
                {
                    case 0:
                        ChangeLimbMaterial(transform.Find("hands/left_hand"));
                        break;
                    case 1:
                        ChangeLimbMaterial(transform.Find("hands/right_hand"));
                        break;
                    case 2:
                        ChangeLimbMaterial(transform.Find("feet/foot_left"));
                        break;
                    case 3:
                        ChangeLimbMaterial(transform.Find("feet/foot_right"));
                        break;

                }
            }
        }
    }

    //Called when enough axe hits are delivered, amputates the arm and updates status
    public void Amputate()
    {
        Transform leftHandTransform = transform.Find("hands/left_hand");
        if (leftHandTransform != null)
        {
            //Give arm gravity
            Rigidbody rb = leftHandTransform.gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;

            //Detach
            leftHandTransform.parent = null;

            //Update status
            status.needsAmputation = false;
            status.needsLeftHand = true;


            if (Time.time - lastVoiceTime >= voiceCooldownTime)
            {
                alienVoice.SayLine("Thanks for cutting it off!");
                lastVoiceTime = Time.time; // Update the time the voice line was last played
            }
 

            //Despawn hand in 10s. Could be changed so that players have to bin the arm.
            Destroy(leftHandTransform.gameObject, 10f);
            //Now needs a new hand
            initiateAttachHand();
        }
        else
        {
            Debug.LogError("left_hand not found under hands");
        }

    }

    private void initiateAttachHand()
    {
        //Remove amputation script
        Transform amputationDetectorTransform = transform.Find("AmputationDetector");
        if (amputationDetectorTransform != null)
        {
            // Get the AmputationBehaviour component from the child object
            AmputationBehaviour amputationBehaviour = amputationDetectorTransform.GetComponent<AmputationBehaviour>();
            if (amputationBehaviour != null)
            {
                // Remove the component (script) from the object
                Destroy(amputationBehaviour);
            }
            else
            {
                Debug.LogWarning("AmputationBehaviour component not found on AmputationDetector.");
            }
        }
        else
        {
            Debug.LogWarning("Child object 'AmputationDetector' not found.");
        }

        //Add attachment script. Conserve collider
        amputationDetectorTransform.gameObject.AddComponent<AttachHand>();

    }

    public void attachHand(GameObject hand)
    {
        Transform amputationDetector = transform.Find("AmputationDetector");
        if (amputationDetector != null)
        {
            // Delete the child object.
            Destroy(amputationDetector.gameObject);
        }
        else
        {
            Debug.LogWarning("Child object 'AmputationDetector' not found.");
        }
        Destroy(hand.transform.Find("ISDK_DistanceHandGrabInteraction").gameObject); //Prevent hand staying grabbable
        Destroy(hand.GetComponent<Rigidbody>()); //Prevent gravity working on hand prior to attachment

        hand.transform.SetParent(transform);
        hand.transform.localScale = Vector3.one; //Adjust scale


        // Set the local position and rotation to the specified values.
        hand.transform.localPosition = new Vector3(-0.0303f, 0.0333f, 0.0004f);
        hand.transform.localRotation = Quaternion.Euler(154.596f, -28.75101f, -4.377991f);

        status.needsLeftHand = false;
        if (status.isHealthy())
        {
            Cure();
        }
        else
        {
            sweatParticles.Stop();
            alienVoice.SayLine("Thanks for the new hand!");
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

    void EnlargementPilled(){

        if(status.curSize == -1){
            status.curSize+=1;
            growthSFX.Play();
            StartCoroutine(ScaleOverTime(2f, growthSFX.clip.length));
        }else if(status.curSize == 0){
            status.curSize+=1;
            growthSFX.Play();
            StartCoroutine(ScaleOverTime(1.4f, growthSFX.clip.length));
        }

        if(status.curSize == 0){
            if(status.isHealthy()){
                Cure();
            }
        }
    }

    void ShrinkPilled(){

        if(status.curSize == 1){
            status.curSize-=1;
            shrinkSFX.Play();
            StartCoroutine(ScaleOverTime((1/(1.4f)), shrinkSFX.clip.length));
        }else if(status.curSize == 0){
            status.curSize-=1;
            shrinkSFX.Play();
            StartCoroutine(ScaleOverTime(0.5f, shrinkSFX.clip.length));
        }

        if(status.curSize == 0){
            if(status.isHealthy()){
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

    public float GetRemainingTime(){
        return timer;
    }

    public int GetInjuryCount(){
        int injuries = 0;
        if (status.needsInjection) injuries++;
        if (status.needsExtinguishing) injuries++;
        if (status.hasShrapnel) injuries ++;
        return injuries;
    }
}