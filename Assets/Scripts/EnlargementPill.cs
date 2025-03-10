using UnityEngine;

public class EnlargementPill : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.CompareTag("Alien")){
            collision.gameObject.SendMessage("EnlargementPilled");
            Destroy(gameObject);
        }
    }

}
