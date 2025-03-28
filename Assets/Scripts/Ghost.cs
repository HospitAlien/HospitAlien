using UnityEngine;
using System.Collections;
public class Ghost : MonoBehaviour
{
    public float speed = 2f;
    public AudioSource audioSource;

    public AudioClip attackVoiceLine;
    public AudioClip killedVoiceLine;

    private GameManager gameManager;
    private bool alive;

    void Start() 
    {
        alive = true;
        gameManager = FindFirstObjectByType<GameManager>();
    }


    void Update()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 directionToCamera = (cameraPosition - transform.position).normalized;
        transform.position += directionToCamera * speed * Time.deltaTime;

        if (directionToCamera.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        float distance = Vector3.Distance(cameraPosition, transform.position);
        if(distance < 0.01)
        {
            Attack();
        }
    }

    void Attack() //the attack will cause a spooky voice line and kill the ghost
    {
        if(alive)
        {
            audioSource.PlayOneShot(attackVoiceLine);
            gameManager.AttackedByGhost();
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
            StartCoroutine(DestroyAfterSound(attackVoiceLine));
            alive = false;
        }


    }

    public void GhostKilled() //if the ghost is killed we want to reward points to the player
    {
        if(alive)
        {
            audioSource.PlayOneShot(killedVoiceLine);
            gameManager.KilledGhost();
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
            StartCoroutine(DestroyAfterSound(killedVoiceLine));
            alive = false;
        }

    }

    private IEnumerator DestroyAfterSound(AudioClip clip)
    {
        // Wait until the sound finishes playing
        yield return new WaitForSeconds(clip.length);
        
        // Destroy the game object
        Destroy(gameObject);
    }

    
    
}
