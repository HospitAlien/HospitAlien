using UnityEngine;

public class ShrapnelBehaviour : MonoBehaviour
{

    private bool insideAlien;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        insideAlien = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!insideAlien)
        { //also want to check that it is not being held
            Destroy(gameObject);
        }

    }

    void OnTriggerExit(Collider collider)
    {
        if (!collider.CompareTag("Shrapnel"))
        {
            AlienBehaviour alien = collider.GetComponentInParent<AlienBehaviour>();
            if (alien != null)
            {
                alien.shrapnelRemoved();
                insideAlien = false;
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag("Shrapnel"))
        {
            AlienBehaviour alien = collider.GetComponentInParent<AlienBehaviour>();
            if (alien != null)
            {
                alien.shrapnelInserted();
                insideAlien = true;
            }
        }
    }
}
