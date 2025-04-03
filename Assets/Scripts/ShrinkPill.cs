using UnityEngine;

public class ShrinkPill : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Alien"))
        {
            Alien alien = collision.gameObject.GetComponent<Alien>();

            if (alien != null)
            {
                if (alien.ShrinkPilled()) Destroy(gameObject);
            }
        }
    }
}
