using UnityEngine;
using System.Collections;
public class GhostEvent : MonoBehaviour
{
    public GameObject ghost;

    void Start() //when the game starts we want to give the user Guns
    {
        StartCoroutine(SpawnGhosts());
    }

    IEnumerator SpawnGhosts()
    {
        int timeToSpawn = Random.Range(0,6);

        while (true)
        {
            SpawnGhost();
            timeToSpawn = Random.Range(0,6);
        }
    }



    private void SpawnGhost()
    {
        Vector3 spawnPosition = Random.onUnitSphere*10;
        spawnPosition += Camera.main.transform.position;
        Instantiate(ghost);
    }
}
