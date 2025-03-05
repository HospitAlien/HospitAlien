using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    private GlobalVariableManager gvm;
    public int score;
    public int maxPatients = 6;
    public int currentPatientCount = 0;
    private bool[] spotOccupied;
    public GameObject patientPrefab;
    private Transform[] spawnLocations;
    public Transform AlienSpawnPoint;
    // private bool _gamePlaying = false;


    //the event state indicates the current event, if it is 0 it means there is no ongoing event, if it is 1 is it pizza time
    private int eventState = 0;
    public GameObject pizzaPrefab;


    public TextMeshPro scoreText;
    private string scoreString = "Money: £%0";

    //I want to have



    void UpdateScoreText()
    {
        scoreText.text = scoreString.Replace("%0", score.ToString());
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
        spawnLocations[3].position = new Vector3(-2, 2, -2);  // Position 4

        spawnLocations[4] = new GameObject("SpawnPoint5").transform;
        spawnLocations[4].position = new Vector3(-2, 2, 0);  // Position 5

        spawnLocations[5] = new GameObject("SpawnPoint6").transform;
        spawnLocations[5].position = new Vector3(-2, 2, 2);  // Position 6

        // Start the game manually for debug, comment this line in production
        StartCoroutine(SpawnPatients());
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

    IEnumerator eventRoutine()
    {

        while (true)
        {
            //we first want to find out what event is going to happen 
            System.Random random = new System.Random();
            int newEvent = random.Next(1, 1);

            //need to find the time before the next event, should happen every 2/3 minutes
            int waitTime = random.Next(120, 180);
            yield return new WaitForSeconds(waitTime);

            eventState = newEvent;
            Debug.Log("waiting for patients to despawn");
            if (newEvent == 1)
            {
                yield return new WaitUntil(() => currentPatientCount == 0); //gotta wait till no aliens are around before we start the event
                yield return StartCoroutine(PizzaTime());
            }


            eventState = 0;
        }
    }

    IEnumerator PizzaTime()
    {
        List<GameObject> spawnedPizzas = new List<GameObject>();
        System.Random random = new System.Random();

        //the pizza event is on for 30s It spawns pizza throughout the room this can be eaten by the doctor to earn coins
        float pizzaEventDuration = 30f;
        float timeElapsed = 0f;

        while (timeElapsed < pizzaEventDuration)
        {
            if (Random.Range(0f, 1f) > 0.5f) // 50% chance pizza
            {

                Vector3 randomPosition = new Vector3(
                    Random.Range(-10f, 10f),
                    1f,
                    Random.Range(-10f, 10f)
                );

                GameObject pizza = Instantiate(pizzaPrefab, randomPosition, Quaternion.identity);
                spawnedPizzas.Add(pizza);
            }

            yield return new WaitForSeconds(0.4f);

            timeElapsed += 0.4f;
        }


        //once the loop ends we need to delete all the pizzas
        foreach (GameObject pizza in spawnedPizzas)
        {
            Destroy(pizza);
        }
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
                    SpawnPatient();
                }
                else
                {
                    // Wait 10 seconds, then try to spawn with 30% chance this percentage in the future will change depending on factors
                    yield return new WaitForSeconds(1f);

                    float chance = Random.Range(0f, 1f);
                    if (chance <= 0.06f)
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
        Transform spawnPoint = spawnLocations[newSpot];


        GameObject patient = Instantiate(patientPrefab, AlienSpawnPoint.transform.position, Quaternion.identity);

        // Access the AlienBehaviour (or equivalent) script on the newly spawned patient and set its target
        AlienBehaviour patientBehaviour = patient.GetComponent<AlienBehaviour>();

        if (patientBehaviour != null)
        {
            // Set the target location for the patient to move towards
            patientBehaviour.SetTarget(spawnPoint);
            patientBehaviour.SetIndex(newSpot);
        }


        // Increment the patient count
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
        score -= 200; //Every time an alien dies the score is reduced
        UpdateScoreText();
    }

    public void PizzaEaten()
    {
        Debug.Log("Pizza slice has been eaten, reward some score");
        score += 50;
        UpdateScoreText();
    }


}
