using UnityEngine;

public class AmputationBehaviour : MonoBehaviour
{
    private AlienBehaviour alienBehaviour;
    private int hitCount = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        alienBehaviour = GetComponentInParent<AlienBehaviour>();
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        CreateCollider();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Counts up hits.
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Axe"))
        {
        hitCount++;
        Debug.Log("Hit by axe: " + hitCount);

        if (hitCount >= 5)
        {
            alienBehaviour.Amputate();
        }
        }
    }

    public void CreateCollider()
    {
        // Create the first BoxCollider with a specific center
        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.center = new Vector3(-0.62f, 0.81f, -0.04f);
        collider.size = new Vector3(0.22f, 0.08f, 0.23f);
    }

}
