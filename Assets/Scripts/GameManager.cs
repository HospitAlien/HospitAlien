using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Oculus.Voice;

public class GameManager : MonoBehaviour
{
    private GlobalVariableManager gvm;

    public int currentGameStage;

    public int score;
    public int maxPatients = 6;
    public int currentPatientCount = 0;

    public float gameLength = 480f; // this is 8 minutes can change

    public GameObject purpleAlienPrefab;
    public GameObject greenAlienPrefab;
    public GameObject orangeAlienPrefab;

    public GhostEvent ghostEvent;
    public GameObject pizzaPrefab;
    public GameObject EventCanvas;
    public GameObject Portal;
    private EventTextController eventTextController;
    // private bool _gamePlaying = false;

    public ScoreBoardManager scoreBoard;
    public GameObject[] endGameObjects;
    public FinishGameMenuManager finishGameMenu;
    private Coroutine restartCoroutine;
    public PassthroughProvider passthroughProvider;

    //the event state indicates the current event, if it is 0 it means there is no ongoing event, if it is 1 is it pizza time

    private List<GameObject> spawnedAliens = new List<GameObject>();
    public GameObject[] spawnLocations;
    private int eventState = 0;
    private bool[] spotOccupied;
    public float restartTime = 90.0f;
    public float waitTimeBeforeStart = 10.0f;
    //I want to have

    public AppVoiceExperience VoiceExperience;

    public float distanceMultiplier = 2.25f;  // Multiplier to adjust ray length
    public float rayOriginYOffset = 0.2f; // Adjust this value in the Inspector to shift the ray down

    private Camera mainCam;
    AlienVoice alienVoice;



    public void UpdateScoreText()
    {
        scoreBoard.setScore(score);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        gvm.OnGamePlayingChangedEvent += GameStatusController;

        score = 0;
        UpdateScoreText();
        spotOccupied = new bool[maxPatients]; //Initially these will all be false


        eventTextController = EventCanvas.GetComponent<EventTextController>();
        EventCanvas.SetActive(false);
        foreach (GameObject obj in endGameObjects)
        {
            obj.SetActive(false);
        }
        StartCoroutine(StartGameCountdown(waitTimeBeforeStart));

        //Warm up connection to NLP (Wit.ai) for voice interation
        GameObject VoiceExperienceObject = GameObject.Find("App Voice Experience");
        if (VoiceExperienceObject != null)
        {
            VoiceExperience = VoiceExperienceObject.GetComponent<AppVoiceExperience>();
        }
        else
        {
            Debug.LogError("VoiceExperienceObject not found");
        }

        VoiceExperience.Activate("Warm up");
        Debug.Log("Warming up connection to Wit.ai");

        mainCam = Camera.main;
    }

    void Update()
    {
        Vector3 origin = mainCam.transform.position + new Vector3(0, -rayOriginYOffset, 0);
        Vector3 direction = mainCam.transform.forward * distanceMultiplier;

        // Draw a red ray for debugging, visible in the Scene view
        Debug.DrawRay(origin, direction, Color.red);
        HandleRaycastAndVisuals();
    }

    public void GameStatusController(bool gamePlaying)
    {
        if (gamePlaying)
        {
            StartGame();
        }
        else
        {
            EndGame();
        }
    }

    IEnumerator StartGameCountdown(float countdownTime)
    {
        float startTime = Time.time;
        while (Time.time - startTime < countdownTime)
        {
            scoreBoard.SetTimeBeforeStart(countdownTime - (Time.time - startTime));
            yield return new WaitForSeconds(0.5f);
        }
        gvm.IsGamePlaying = true;
        foreach (GameObject obj in endGameObjects)
        {
            obj.SetActive(true);
        }
    }

    public void StartRestartCountdown()
    {
        if (restartCoroutine != null)
        {
            StopCoroutine(restartCoroutine);
        }
        restartCoroutine = StartCoroutine(RestartGameCountdown(90.0f));
    }

    IEnumerator RestartGameCountdown(float countdownTime)
    {
        float startTime = Time.time;
        while (Time.time - startTime < countdownTime)
        {
            finishGameMenu.SetRestartTime(countdownTime - (Time.time - startTime));
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("Restarting game");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Interactive_tutorial");
    }

    private void StartGame()
    {
        score = 0;
        currentGameStage = 0;
        UpdateScoreText();
        currentPatientCount = 0;
        eventState = 0;
        spotOccupied = new bool[maxPatients];

        StartCoroutine(SpawnPatients());
        StartCoroutine(eventRoutine());
        StartCoroutine(timeRemaining());
    }


    private void EndGame()
    {
        StartCoroutine(gvm.LoadLeaderboardData());
        currentPatientCount = 0;
        eventState = 0;
        spotOccupied = new bool[maxPatients];
        StopAllCoroutines(); // Stop all coroutines first
        DeleteObjectsWithScript<Alien>();
        DeleteObjectsWithScript<PizzaScript>();
        scoreBoard.HideTime();
        foreach (GameObject obj in endGameObjects)
        {
            obj.SetActive(false);
        }
        EventCanvas.SetActive(true);
        eventTextController.StartCongratulations();
        finishGameMenu.OpenMenu();
        finishGameMenu.SetScoreText(score);
        passthroughProvider.TogglePassThrough(true);
        StartRestartCountdown();
    }


    public void DeleteObjectsWithScript<T>() where T : MonoBehaviour
    {
        T[] objectsWithScript = FindObjectsByType<T>(FindObjectsSortMode.None);
        foreach (T obj in objectsWithScript)
        {
            Destroy(obj.gameObject);
        }
    }

    IEnumerator timeRemaining()
    {
        float gameStartTime = Time.time;
        while (Time.time - gameStartTime < gameLength)
        {
            scoreBoard.setTime(gameLength - (Time.time - gameStartTime));
            yield return new WaitForSeconds(0.5f);
        }
        gvm.IsGamePlaying = false;
    }


    IEnumerator eventRoutine()
    {

        while (currentGameStage < 2)
        {
            int waitTime;
            if (currentGameStage == 0)
            {
                waitTime = 90;
            }
            else
            {
                waitTime = 120;
            }
            yield return new WaitForSeconds(waitTime);

            eventState = 1; //event on going, stops aliens spawning
            yield return new WaitUntil(() => currentPatientCount == 0); //gotta wait till no aliens are around before we start the event

            if (currentGameStage == 0)
            {
                yield return StartCoroutine(PizzaTime());
            }
            else
            {
                EventCanvas.SetActive(true);
                eventTextController.SetEventText("Ghosts are attacking! Grab the gun!");
                eventTextController.SetEventColor(Color.yellow);
                yield return StartCoroutine(ghostEvent.StartEvent());
            }

            EventCanvas.SetActive(false);
            eventState = 0;

            currentGameStage++;
        }
    }

    IEnumerator PizzaTime()
    {
        List<GameObject> spawnedPizzas = new List<GameObject>();

        EventCanvas.SetActive(true);
        eventTextController.SetEventText("Lunch Time! Grab And Eat Pizza!");
        eventTextController.SetEventColor(Color.yellow);

        //the pizza event is on for 30s It spawns pizza throughout the room this can be eaten by the doctor to earn coins
        float pizzaEventDuration = 30f;
        float pizzaTimeElapsed = 0f;

        while (pizzaTimeElapsed < pizzaEventDuration)
        {

            Vector3 randomPosition = new Vector3(
                Random.Range(-8f, 8f),
                1f,
                Random.Range(-8f, 8f)
            );

            GameObject pizza = Instantiate(pizzaPrefab, randomPosition, Quaternion.identity);
            Rigidbody rb = pizza.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                float randomForceMagnitude = Random.Range(1f, 1.3f);
                rb.AddForce(randomDirection * randomForceMagnitude, ForceMode.Impulse);

                Vector3 randomTorque = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
                rb.AddTorque(randomTorque, ForceMode.Impulse);
            }
            spawnedPizzas.Add(pizza);
            yield return new WaitForSeconds(0.5f);

            pizzaTimeElapsed += 0.5f;
        }
        //once the loop ends we need to delete all the pizzas
        foreach (GameObject pizza in spawnedPizzas)
        {
            Destroy(pizza);
        }
        eventState = 0;
    }



    IEnumerator SpawnPatients()
    {
        while (true)
        {
            if (currentPatientCount < 6 && eventState == 0)
            {

                if (currentPatientCount == 0) //if there are no patients we should spawn one in 2 seconds
                {
                    yield return new WaitForSeconds(2f);
                    if (eventState == 0)
                    {
                        SpawnPatient();
                    }
                }
                else
                {
                    // Wait 10 seconds, then try to spawn with 30% chance this percentage in the future will change depending on factors
                    yield return new WaitForSeconds(1f);

                    float chance = Random.Range(0f, 1f);
                    float spawnChance = 0.08f;
                    if (currentGameStage == 0) spawnChance = 0.05f;

                    if (chance <= spawnChance && eventState == 0)
                    {
                        SpawnPatient();
                    }
                }
            }
            else
            { //otherwise we dont want to overwelm the program so we will wait a second before checking again
                yield return new WaitForSeconds(1f);
            }
        }
    }

    void SpawnPatient()
    {
        // Find all unoccupied spots
        List<int> availableSpots = new List<int>();

        for (int i = 0; i < spotOccupied.Length; i++)
        {
            if (!spotOccupied[i])
            {
                availableSpots.Add(i);
            }
        }


        int newSpot = availableSpots[Random.Range(0, availableSpots.Count)];
        spotOccupied[newSpot] = true;


        // Select a random spawn location from the spawnLocations array
        GameObject spawnPoint = spawnLocations[newSpot];
        GameObject patient;
        int alienType = Random.Range(1, 4);

        if (alienType == 1)
        {
            patient = Instantiate(purpleAlienPrefab, Portal.transform.position + new Vector3(1, 0, 0), Quaternion.identity);
        }
        else if (alienType == 2)
        {
            patient = Instantiate(greenAlienPrefab, Portal.transform.position + new Vector3(1, 0, 0), Quaternion.identity);
        }
        else
        {
            patient = Instantiate(orangeAlienPrefab, Portal.transform.position + new Vector3(1, 0, 0), Quaternion.identity);
        }

        // Access the AlienBehaviour (or equivalent) script on the newly spawned patient and set its target
        Alien patientBehaviour = patient.GetComponent<Alien>();

        if (patientBehaviour != null)
        {
            // Set the target location for the patient to move towards
            patientBehaviour.SetTarget(spawnPoint);
            patientBehaviour.SetIndex(newSpot);
        }

        spawnedAliens.Add(patient);


        // Increment the patient count
        currentPatientCount++;

    }



    public void PatientCured(int patientIndex, int reward)
    {
        Debug.Log("Alien with index " + patientIndex + " has been cured.");
        spotOccupied[patientIndex] = false;
        currentPatientCount--;
        score += reward;
        UpdateScoreText();
    }

    public void PatientDied(int patientIndex)
    {
        spotOccupied[patientIndex] = false;
        currentPatientCount--;
        UpdateScoreText();
    }

    public void PizzaEaten()
    {
        score += 50;
        UpdateScoreText();
    }

    public void AttackedByGhost()
    {
        score -= 50;
        score = System.Math.Max(score, 0);
        UpdateScoreText();
    }

    public void KilledGhost()
    {
        score += 25;
        UpdateScoreText();
    }




    //exposing stuff to music manager
    public int GetNumberOfPatients()
    {
        return currentPatientCount;
    }

    public int GetEvent()
    {
        return eventState;
    }

    // Returns the total number of injuries by summing each active alien's injury count.
    public int GetTotalInjuries()
    {
        int total = 0;
        var aliens = FindObjectsByType<Alien>(FindObjectsSortMode.None);
        foreach (Alien alien in aliens)
        {
            total += alien.GetInjuryCount();
        }
        return total;
    }

    // Returns the total remaining time from all active aliens.
    public float GetTotalTimeLeft()
    {
        float total = 0f;
        var aliens = FindObjectsByType<Alien>(FindObjectsSortMode.None);
        foreach (Alien alien in aliens)
        {
            total += Mathf.Pow(60 - alien.GetRemainingTime(), 1.3f);
        }
        return total;
    }

    void OnDestroy()
    {
        gvm.OnGamePlayingChangedEvent -= GameStatusController;
        if (restartCoroutine != null)
        {
            StopCoroutine(restartCoroutine);
        }
    }

    void HandleRaycastAndVisuals()
    {

        // Set the ray's origin to the camera's current position
        Vector3 origin = mainCam.transform.position + new Vector3(0, -rayOriginYOffset, 0);
        // Set the ray's direction to where the camera is currently facing
        Vector3 direction = mainCam.transform.forward;

        // Set a default end position at the maximum ray length from the origin
        Vector3 endPosition = origin + direction * distanceMultiplier;

        // Perform the raycast using the calculated origin, direction, and maximum distance,
        // while filtering by the specified layer mask.
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, distanceMultiplier))
        {
            // If the ray hits an object, update the end position to the hit point
            endPosition = hit.point;

            // Check if the hit object has the tag "NPC"
            if (hit.collider.CompareTag("Head"))
            {
                alienVoice = hit.collider.GetComponentInParent<AlienVoice>();
                if (alienVoice != null)
                {
                    // Call the desired function on the Alien script
                    if (!VoiceExperience.Active)
                    {
                        alienVoice.ActivateListening();
                    }
                }
                else
                {
                    Debug.LogError("Alien voice not found.");
                }
            }
            else
            {
                if (VoiceExperience.Active)
                {
                    if (alienVoice != null)
                    {
                        alienVoice.Deactivate();
                    }
                }
            }
        }
        else
        {
            if (VoiceExperience.Active)
            {
                if (alienVoice != null)
                {
                    alienVoice.Deactivate();
                }
            }
        }
    }
}
