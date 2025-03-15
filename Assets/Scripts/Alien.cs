using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;


public class Alien : MonoBehaviour
{
    protected Transform target;
    protected int index;
    public Vector3 targetLocation;

    protected float timer = 120;
    protected TextMesh timerText;

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

    protected float lastVoiceTime = -Mathf.Infinity;
    protected float voiceCooldownTime = 3f;

    protected Status status;
    protected AlienVoice alienVoice;
    protected int reward = 0;

    protected Dictionary<Vector3, (Vector3, Quaternion)> beds
        = new Dictionary<Vector3, (Vector3, Quaternion)>();



    void Start()
    {
        status = new Status();

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

    protected virtual void InitiateStatus(){
        Debug.Log("initiate the status of the alien");
    }


    protected void InitiateTimer()
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

    public string getVoiceLine()
    {
        return status.getIllness();
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

    protected void Syrined()
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


    public void SetTarget(Transform newTarget)
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
        Destroy(gameObject);
    }
}
