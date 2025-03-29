using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    // If you're not using GlobalVariableManager (or VR controller events), you can comment out these lines.
    // private GlobalVariableManager gvm;

    public int score;
    public int maxPatients = 6;
    public int currentPatientCount = 0;
    public GameObject purpleAlienPrefab;
    public GameObject greenAlienPrefab;

    public GameObject pizzaPrefab;
    public GameObject EventCanvas;
    public GameObject Portal;
    private EventTextController eventTextController;
    // private bool _gamePlaying = false;

    public ScoreBoardManager scoreBoard;

    // The event state indicates the current event: 0 means no event, 1 would mean a pizza event.
    private Transform[] spawnLocations;
    private int eventState = 0;
    private bool[] spotOccupied;

    void UpdateScoreText()
    {
        //scoreBoard.setScore(score);
        Debug.Log("update score attempt");
    }

    // Start is called once before the first frame update
    void Start()
    {
        // If not using GlobalVariableManager or VR input, comment out these lines:
        // gvm = FindFirstObjectByType<GlobalVariableManager>();
        // gvm.OnGamePlayingChangedEvent += GameStatusController;

        score = 0;
        UpdateScoreText();
        spotOccupied = new bool[maxPatients]; // Initially, these will all be false

        // Define 6 spawn locations programmatically.
        spawnLocations = new Transform[6];

        spawnLocations[0] = new GameObject("SpawnPoint1").transform;
        spawnLocations[0].position = new Vector3(2, 2, -2);

        spawnLocations[1] = new GameObject("SpawnPoint2").transform;
        spawnLocations[1].position = new Vector3(2, 2, 0);

        spawnLocations[2] = new GameObject("SpawnPoint3").transform;
        spawnLocations[2].position = new Vector3(2, 2, 2);

        spawnLocations[3] = new GameObject("SpawnPoint4").transform;
        spawnLocations[3].position = new Vector3(0, 2, -2);

        spawnLocations[4] = new GameObject("SpawnPoint5").transform;
        spawnLocations[4].position = new Vector3(0, 2, 2);

        spawnLocations[5] = new GameObject("SpawnPoint6").transform;
        spawnLocations[5].position = new Vector3(-2, 2, 2);

        eventTextController = EventCanvas.GetComponent<EventTextController>();
        EventCanvas.SetActive(false);

        // For debugging / testing patient spawning, we call only the SpawnPatients coroutine.
        StartCoroutine(SpawnPatients());
        // Comment out the event routine if you don't need it:
        // StartCoroutine(eventRoutine());
    }

    // We are not using the GameStatusController method in this test setup.
    /*
    public void GameStatusController(bool gamePlaying)
    {
        if (gamePlaying)
        {
            // _gamePlaying = true;
            StartCoroutine(SpawnPatients());
            StartCoroutine(eventRoutine());
        }
        else
        {
            // _gamePlaying = false;
            StopAllCoroutines();
        }
    }
    */

    // Event routines can be commented out if not needed.
    /*
    IEnumerator eventRoutine()
    {
        while (true)
        {
            System.Random random = new System.Random();
            int newEvent = random.Next(1, 1); // Note: This line may throw an exception. Adjust if needed.
            int waitTime = random.Next(100, 140);
            yield return new WaitForSeconds(waitTime);
            eventState = newEvent;
            Debug.Log("waiting for patients to despawn");
            if (newEvent == 1)
            {
                yield return new WaitUntil(() => currentPatientCount == 0);
                StartCoroutine(PizzaTime());
                yield return new WaitUntil(() => eventState == 0);
            }
        }
    }

    IEnumerator PizzaTime()
    {
        List<GameObject> spawnedPizzas = new List<GameObject>();
        EventCanvas.SetActive(true);
        eventTextController.SetEventText("Lunch Time! Grab And Eat Pizza!");
        eventTextController.SetEventColor(Color.yellow);
        float pizzaEventDuration = 30f;
        float timeElapsed = 0f;

        while (timeElapsed < pizzaEventDuration)
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
            timeElapsed += 0.5f;
        }
        EventCanvas.SetActive(false);
        foreach (GameObject pizza in spawnedPizzas)
        {
            Destroy(pizza);
        }
        eventState = 0;
    }
    */

    IEnumerator SpawnPatients()
    {
        Debug.Log("started spawning");
        while (true)
        {
            if (currentPatientCount < 6 && eventState == 0)
            {
                if (currentPatientCount == 0) // if there are no patients, spawn one after 5 seconds
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
                    // Wait 1 second, then with a 6% chance spawn a patient.
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
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    void SpawnPatient()
    {
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

        Transform spawnPoint = spawnLocations[newSpot];
        GameObject patient;
        int alienType = Random.Range(1, 3);
        if (alienType == 1)
        {
            patient = Instantiate(purpleAlienPrefab, Portal.transform.position + new Vector3(1, 0, 0), Quaternion.identity);
        }
        else
        {
            patient = Instantiate(greenAlienPrefab, Portal.transform.position + new Vector3(1, 0, 0), Quaternion.identity);
        }

        Alien patientBehaviour = patient.GetComponent<Alien>();
        if (patientBehaviour != null)
        {
            patientBehaviour.SetTarget(spawnPoint);
            patientBehaviour.SetIndex(newSpot);
        }

        currentPatientCount++;
    }

    public void PatientCured(int patientIndex, int reward = 100)
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

    // Exposing stuff to the music manager.
    public int GetNumberOfPatients()
    {
        return currentPatientCount;
    }

    public int GetEvent()
    {
        return eventState;
    }

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

    public float GetTotalTimeLeft()
    {
        float total = 0f;
        var aliens = FindObjectsByType<Alien>(FindObjectsSortMode.None);
        foreach (Alien alien in aliens)
        {
            total += Mathf.Pow(60-alien.GetRemainingTime(), 1.3f);
        }
        return total;
    }
}
