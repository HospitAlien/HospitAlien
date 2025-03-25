using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

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

    public GameObject pizzaPrefab;
    public GameObject EventCanvas;
    public GameObject Portal;
    private EventTextController eventTextController;
    // private bool _gamePlaying = false;

    public ScoreBoardManager scoreBoard;

    //the event state indicates the current event, if it is 0 it means there is no ongoing event, if it is 1 is it pizza time

    private List<GameObject> spawnedAliens = new List<GameObject>();
    private Transform[] spawnLocations;
    private int eventState = 0;
    private bool[] spotOccupied;
    //I want to have


    void UpdateScoreText()
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



        // Define 6 spawn locations programmatically
        spawnLocations = new Transform[6];  // Array of 6 spawn locations

        // Defining spawn points at specific positions
        spawnLocations[0] = new GameObject("SpawnPoint1").transform;
        spawnLocations[0].position = new Vector3(2, 2, -2);  // Position 1

        spawnLocations[1] = new GameObject("SpawnPoint2").transform;
        spawnLocations[1].position = new Vector3(2, 2, 0);  // Position 2

        spawnLocations[2] = new GameObject("SpawnPoint3").transform;
        spawnLocations[2].position = new Vector3(2, 2, 2);  // Position 3

        spawnLocations[3] = new GameObject("SpawnPoint4").transform;
        spawnLocations[3].position = new Vector3(0, 2, -2);  // Position 4

        spawnLocations[4] = new GameObject("SpawnPoint5").transform;
        spawnLocations[4].position = new Vector3(0, 2, 2);  // Position 5

        spawnLocations[5] = new GameObject("SpawnPoint6").transform;
        spawnLocations[5].position = new Vector3(-2, 2, 2);  // Position 6

        eventTextController = EventCanvas.GetComponent<EventTextController>();
        EventCanvas.SetActive(false);

        // Start the game manually for debug, comment this line in production
        // StartCoroutine(SpawnPatients());
        // StartCoroutine(eventRoutine());
    }



    // Update is called once per frame
    // void Update()
    // {

    // }

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

    private void StartGame()
    {
        score = 0;
        currentGameStage = 0;
        UpdateScoreText();
        currentPatientCount = 0;
        eventState = 0;
        spotOccupied = new bool[maxPatients];
        EventCanvas.SetActive(false);

        StartCoroutine(SpawnPatients());
        StartCoroutine(eventRoutine());
        StartCoroutine(timeRemaining());
    }


    private void EndGame()
    {
        currentPatientCount = 0;
        eventState = 0;
        spotOccupied = new bool[maxPatients];
        EventCanvas.SetActive(false);

        DeleteObjectsWithScript<Alien>();
        DeleteObjectsWithScript<PizzaScript>();

        StopAllCoroutines();
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
        while(Time.time - gameStartTime < gameLength)
        {
            scoreBoard.setTime(gameLength -(Time.time - gameStartTime));
            yield return new WaitForSeconds(1f);
        }
        gvm.IsGamePlaying = false;

        //need to change the buton idk how to do that
    }


    IEnumerator eventRoutine()
    {

        while (currentGameStage < 2)
        {
            int waitTime;
            if(currentGameStage == 0) {
                waitTime = 90;
            }else{
                waitTime = 120;
            }
            yield return new WaitForSeconds(waitTime);

            eventState = 1; //pizza time
            Debug.Log("waiting for patients to despawn");
            yield return new WaitUntil(() => currentPatientCount == 0); //gotta wait till no aliens are around before we start the event
            Debug.Log($"NO PATIENTS LEFT {currentPatientCount}");
            yield return StartCoroutine(PizzaTime());
            Debug.Log("Piza time finished");
            currentGameStage++;
        }
    }

    IEnumerator PizzaTime()
    {
        Debug.Log($"Pizza time starting, Current patient count: {currentPatientCount }");
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
                Random.Range(-10f, 10f),
                1f,
                Random.Range(-10f, 10f)
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

        EventCanvas.SetActive(false);
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

                if (currentPatientCount == 0) //if there are no patients we should spawn one in 5 seconds
                {
                    yield return new WaitForSeconds(5f);
                    if (eventState == 0)
                    {
                        Debug.Log("Spawning patient");
                        SpawnPatient();
                    }
                }
                else
                {
                    // Wait 10 seconds, then try to spawn with 30% chance this percentage in the future will change depending on factors
                    yield return new WaitForSeconds(1f);

                    float chance = Random.Range(0f, 1f);
                    if (chance <= 0.06f && eventState == 0)
                    {
                        Debug.Log("Spawning patient");
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
        Transform spawnPoint = spawnLocations[newSpot];
        GameObject patient;
        int alienType = Random.Range(1, 3);
        if(alienType == 1)
        {
            patient = Instantiate(purpleAlienPrefab, Portal.transform.position + new Vector3(1, 0, 0), Quaternion.identity);
        }
        else
        {
            patient = Instantiate(greenAlienPrefab, Portal.transform.position + new Vector3(1, 0, 0), Quaternion.identity);
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
        Debug.Log("Alien with index " + patientIndex + " has been deleted.");
        spotOccupied[patientIndex] = false;
        currentPatientCount--;
        UpdateScoreText();
    }

    public void PizzaEaten()
    {
        Debug.Log("Pizza slice has been eaten, reward some score");
        score += 50;
        UpdateScoreText();
    }

    public void AttackedByGhost()
    {
        Debug.Log("Ghost attack!");
        score -= 50;
        score = System.Math.Min(score,0);
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
            total += Mathf.Pow(alien.GetRemainingTime(), 1.3f);
        }
        return total;
    }
}
