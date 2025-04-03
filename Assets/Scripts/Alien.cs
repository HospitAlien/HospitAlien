using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Oculus.Interaction;
using TMPro;

public class Alien : MonoBehaviour
{
    protected GameObject target;
    protected int index;

    protected float timer = 80;
    protected TextMeshProUGUI timerText;

    protected static GameManager gameManager;
    protected NavMeshAgent agent;

    protected bool isReady = false;

    public ParticleSystem sweatParticles;
    public ParticleSystem fireParticles;
    public GameObject shrapnel;
    public Transform bodyTransform;
    public GameObject coinParticlePrefab;
    public AudioSource growthSFX;
    public AudioSource shrinkSFX;
    protected Material amputateMaterial;
    protected Material originalLimbMaterial;
    public Animator animator;


    protected float lastVoiceTime = -Mathf.Infinity;
    protected float voiceCooldownTime = 3f;

    public Status status;
    protected AlienVoice alienVoice;
    public int reward = 200;

    public GameObject handPrefab;
    public GameObject legPrefab;


    void Start()
    {
        status = new Status();

        animator = GetComponent<Animator>();

        alienVoice = GetComponent<AlienVoice>();

        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }



        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(target.transform.position);

        InitiateStatus();
        InitiateTimer();
    }

    protected virtual void InitiateStatus(){
        Debug.Log("initiate the status of the alien");
    }


    protected void InitiateTimer()
    {
        // Create a new TextMesh object for displaying the countdown
        timerText = target.GetComponentInChildren<TextMeshProUGUI>();

        Debug.Log("GOT HERE");
        Debug.Log(timerText);


        // Start the countdown coroutine
        StartCoroutine(CountdownTimer());
    }

    protected IEnumerator CountdownTimer()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime; // Decrease the timer by time passed
            timerText.text = Mathf.Ceil(timer).ToString(); // Update the text to show the remaining time
            yield return null; // Wait for the next frame
        }

        // After the countdown, delete the object
        Kill();
    }

    protected void Kill()
    {
        alienVoice.SayLine("You failed me!");
        StopAllCoroutines();
        timerText.text = "";

        gameManager.PatientDied(index);
        Destroy(gameObject);
    }


    void FixedUpdate()
    {
        if (target != null)
        {


            // Check if the agent has reached the destination
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash,0,0f);
                animator.speed = 0f;
                animator.enabled = false;
                agent.enabled = false;


                isReady = true;
                

                // Set the position and rotation to match the target
                transform.position = target.transform.position;
                transform.rotation = target.transform.rotation;
                transform.position += new Vector3(0f,0.5f,0f);
                transform.localPosition += target.transform.forward * -1f; 
                transform.rotation = target.transform.rotation * Quaternion.Euler(-90f, 180f, 0f);


                //Reposition timer so it's not on the floor (it's rotated alongside the alien")
                Transform timerText = transform.Find("TimerText");
                if (timerText != null)
                {
                    timerText.localPosition = new Vector3(0f, 0.15f, 0.06f);
                    timerText.localRotation = Quaternion.Euler(-90f, 180f, 0f);
                }
                target = null;
            }
            else
            {
                agent.SetDestination(target.transform.position);
                isReady = false;
            }
        }
    }

    public string getVoiceLine()
    {
        return status.getIllness();
    }

    public string getBloodType()
    {
        return status.bloodType;
    }

    protected virtual Material LoadLimbMaterial()
    {
        Debug.Log("LoadLimbMaterial must be overridden");
        return null;
    }

    protected void ChangeLimbMaterial(Transform limbTransform)
    {
        // Finds the "left_hand" transform
        if (limbTransform != null)
        {
            Renderer limbRenderer = limbTransform.GetComponent<Renderer>();
            if (limbRenderer != null)
            {
                // Save original limb material for restoration later
                if (originalLimbMaterial == null)
                {
                    originalLimbMaterial = limbRenderer.material;
                }
                //Change limb material to indicate need for amputation
                limbRenderer.material = LoadLimbMaterial();
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
    public virtual void initiateAmputation()
    {
        Debug.Log("InitiateAmputation not overridden");
    }

    public void Amputate(int limb)
    {
        status.amputate(limb);

        Transform limbTransform = null;
        switch (limb)
        {
            case 0:
                limbTransform = transform.Find("body/hands/left_hand");
                break;
            case 1:
                limbTransform = transform.Find("body/hands/right_hand");
                break;
            case 2:
                limbTransform = transform.Find("body/feet/foot_left");
                break;
            case 3:
                limbTransform = transform.Find("body/feet/foot_right");
                break;

        }
       
        if (limbTransform != null)
        {
            // Deactivate the original limb
            limbTransform.gameObject.SetActive(false);

            //Drop a limb to show amputation
            GameObject limbPrefab = null;
            switch (limb)
            {
                case 0:
                   limbPrefab = handPrefab;
                    break;
                case 1:
                   limbPrefab = handPrefab;
                    break;
                case 2:
                    limbPrefab = legPrefab;
                    break;
                case 3:
                    limbPrefab = legPrefab;
                    break;
            }

            GameObject limbClone = Instantiate(limbPrefab, limbTransform.position, limbTransform.rotation);

            if (Time.time - lastVoiceTime >= voiceCooldownTime)
            {
                alienVoice.SayLine("Thanks for cutting it off!");
                lastVoiceTime = Time.time; // Update the time the voice line was last played
            }
 

            //Despawn clone in 10s. Could be changed so that players have to bin the arm.
            Destroy(limbClone.gameObject, 10f);
            //Now needs a new hand
            initiateAttachHand();
        }
        else
        {
            Debug.LogError("left_hand not found under hands");
        }

    }

    protected virtual void initiateAttachHand()
    {
        Debug.Log("initiateAttachHand must be overridden");
    }

    public void attachLimb(GameObject bodyPart, int limb){
        Destroy(bodyPart); //Destroy the donor arm

        //Find and restore limb
        Transform originalLimb = null;
        switch (limb)
        {
            case 0:
                originalLimb = transform.Find("body/hands/left_hand");
                break;
            case 1:
                originalLimb = transform.Find("body/hands/right_hand");
                break;
            case 2:
                originalLimb = transform.Find("body/feet/foot_left");
                break;
            case 3:
                originalLimb = transform.Find("body/feet/foot_right");
                break;
        }

        if (originalLimb != null)
        {
            originalLimb.gameObject.SetActive(true);

            //Restore limb colour
            Renderer renderer = originalLimb.GetComponent<Renderer>();

            if (renderer != null && originalLimbMaterial != null)
            {
                renderer.material = originalLimbMaterial;
            }
            else
            {
                Debug.Log("Either renderer component not found or original limb material not saved");
            }
        }
        else
        {
            Debug.LogError("Original limb not found for reattachment");
        }



        status.attachLimb(limb);

        status.needsAttatchment[limb] = false;

        if (status.isHealthy())
        {
            Cure();
        }
        else
        {
            alienVoice.SayLine("Thanks for the new limb!");
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

    public void Syrined(string syringeBloodType)
    {
        if (isReady)
        {
            if (status.needsInjection && (syringeBloodType == status.bloodType) && syringeBloodType != "")
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
                Kill();
            }
        }
    }

    protected void EnlargementPilled()
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


    protected void ShrinkPilled()
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


    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public void SetIndex(int newIndex)
    {
        index = newIndex;
    }

    public float GetRemainingTime(){
        return timer;
    }

    public int GetInjuryCount(){
        int injuries = 0;
        if (status.needsInjection) injuries++;
        if (status.needsExtinguishing) injuries++;
        if (status.hasShrapnel) injuries++;
        return injuries;
    }

    public void Cure(){
        alienVoice.SayLine("Ah... Much better");
        gameManager.PatientCured(index, reward);
        if (coinParticlePrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0f, 0.5f, 0f);
            GameObject newObject = Instantiate(coinParticlePrefab, spawnPosition, Quaternion.Euler(-90f, 0f, 0f));
            CoinBehaviour theCoins = newObject.GetComponent<CoinBehaviour>();
            theCoins.SetRate(12);
        }

        StopAllCoroutines();
        timerText.text = "";

        Destroy(gameObject);
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
}
