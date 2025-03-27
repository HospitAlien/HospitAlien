using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float speed = 2f;
    public AudioSource audioSource;

    public AudioClip attackVoiceLine;
    public AudioClip killedVoiceLine;

    private GameManager gameManager;

    void Start() 
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }


    void Update()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 directionToCamera = (cameraPosition - transform.position).normalized;
        transform.position += directionToCamera * speed * Time.deltaTime;

        float distance = Vector3.Distance(cameraPosition, transform.position);
        if(distance < 0.01)
        {
            Attack();
        }
    }

    void Attack() //the attack will cause a spooky voice line and kill the ghost
    {
        audioSource.PlayOneShot(attackVoiceLine);
        gameManager.AttackedByGhost();
        Destroy(gameObject);
    }

    public void GhostKilled() //if the ghost is killed we want to reward points to the player
    {
        audioSource.PlayOneShot(killedVoiceLine);
        gameManager.KilledGhost();
        Destroy(gameObject);
    }
    
}
