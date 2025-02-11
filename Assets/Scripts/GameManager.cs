using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
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
    private bool gameStarted = false;


    public TextMeshPro scoreText;
    private string scoreString = "Score: %0";

    void UpdateScoreText()
    {
        scoreText.text = scoreString.Replace("%0", score.ToString());
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();

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

        StartCoroutine(SpawnPatients());
    }

    // Update is called once per frame
    void Update()
    {

    }




    IEnumerator SpawnPatients()
    {
        while (gvm.GameStarted) //in the future this can be changed to while game is running
        {
            if (currentPatientCount < 6)
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
                    if (chance <= 0.1f)
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


        GameObject patient = Instantiate(patientPrefab, new Vector3(-15f, 0.08333334f, 7.5f), Quaternion.identity);

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



    public void PatientCured(int patientIndex)
    {
        Debug.Log("Alien with index " + patientIndex + " has been cured.");
        spotOccupied[patientIndex] = false;
        currentPatientCount--;
        score += 1;
        UpdateScoreText();
    }

    public void PatientDied(int patientIndex)
    {
        Debug.Log("Alien with index " + patientIndex + " has been deleted.");
        spotOccupied[patientIndex] = false;
        currentPatientCount--;
        score -= 1; //Every time an alien dies the score is reduced
        UpdateScoreText();
    }


}
