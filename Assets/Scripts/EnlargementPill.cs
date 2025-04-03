using UnityEngine;

public class EnlargementPill : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Alien"))
        {
            Alien alien = collision.gameObject.GetComponent<Alien>();

            if (alien != null)
            {
                if (alien.EnlargementPilled()) Destroy(gameObject);
            }
        }
    }
}
