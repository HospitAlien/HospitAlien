using UnityEngine;

public class PizzaScript : MonoBehaviour
{
    //this script is basically just checking if the pizza is held up close to the face, if it is then score should be added
    private static GameManager gameManager;
    public GameObject coinParticlePrefab;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

    }

    // Update is called once per frame
    void Update()
    {
    }


    private void PizzaEaten()
    {
        if (coinParticlePrefab != null)
        {
            // Instantiate the particle system at the current position and with the current rotation
            GameObject newObject = Instantiate(coinParticlePrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));
            CoinBehaviour theCoins = newObject.GetComponent<CoinBehaviour>();
            theCoins.SetRate(12);

            Debug.Log("Coins spawned");
        }

        //if the pizza is eaten we want to call the function within gameManager to update score
        Destroy(gameObject);
        gameManager.PizzaEaten();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Pizza collided with " + other.gameObject.name);


        if (other.gameObject.CompareTag("MainCamera"))
        {
            PizzaEaten();
        }
    }
}
