using UnityEngine;

public class VendingButton : MonoBehaviour
{
    public GameObject objectPrefab;

    // Offset relative to the parent (vending machine) position
    public Vector3 spawnOffset = new Vector3(0f, 0.1f, 0f);

    private GameObject currentObject;

    private void OnTriggerEnter(Collider other)
    {
        SpawnObject();
    }

    void SpawnObject()
    {
        // If a syringe already exists, destroy it
        if (currentObject != null)
        {
            Destroy(currentObject);
        }

        // Find the spawn position based on the parent's child "TableLight" position plus an offset
        Vector3 spawnPosition = transform.parent.Find("TableLight").position + spawnOffset;

        currentObject = Instantiate(objectPrefab, spawnPosition, transform.parent.rotation);

        Debug.Log("Syringe spawned at: " + spawnPosition);
    }
}
