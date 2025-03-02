using UnityEngine;

public class VendingButton : MonoBehaviour
{
    // Reference to the syringe prefab (assign this in the Inspector)
    public GameObject syringePrefab;

    // Time interval between spawns (in seconds)
    public float spawnInterval = 5f;

    // Offset relative to the parent (vending machine) position; default is one unit down
    public Vector3 spawnOffset = new Vector3(0f, -1f, 0f);

    void Start()
    {
        // Start spawning immediately and then every spawnInterval seconds
        InvokeRepeating("SpawnSyringe", 0f, spawnInterval);
        Debug.Log("Started");
    }

    // This method instantiates the syringe prefab at the parent's position plus the offset
    void SpawnSyringe()
    {
        if (syringePrefab == null)
        {
            Debug.LogWarning("Syringe prefab is not assigned.");
            return;
        }

        if (transform.parent == null)
        {
            Debug.LogWarning("No parent found for positioning the syringe.");
            return;
        }

        // Calculate the spawn position just beneath the vending machine (parent object)
        Vector3 spawnPosition = transform.parent.position + spawnOffset;

        // Instantiate the syringe prefab with the parent's rotation
        Instantiate(syringePrefab, spawnPosition, transform.parent.rotation);
        Debug.Log("Syringe spawned at: " + spawnPosition);
    }
}
