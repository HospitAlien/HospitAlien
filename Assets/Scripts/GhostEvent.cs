using UnityEngine;
using System.Collections;
public class GhostEvent : MonoBehaviour
{
    public GameObject ghost;


    public IEnumerator StartEvent()   //when the game starts we want t give the user Guns and start ghosts spawning
    {
        yield return StartCoroutine(SpawnGhosts());
    }

    IEnumerator SpawnGhosts()
    {
        float startTime = Time.time;

        while (Time.time - startTime < 30f)
        {
            SpawnGhost();
            int timeToSpawn = Random.Range(0,6);
            yield return new WaitForSeconds(timeToSpawn);
        }
    }



    private void SpawnGhost()
    {
        Vector3 spawnPosition = Random.onUnitSphere*10;
        if (spawnPosition.y < 0f)
        {
            spawnPosition.y = Mathf.Abs(spawnPosition.y);
        }
        spawnPosition += Camera.main.transform.position;
        Instantiate(ghost, spawnPosition, Quaternion.identity);
    }
}
