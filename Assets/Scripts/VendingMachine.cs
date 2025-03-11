using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    // Offset relative to the parent (vending machine) position
    public Vector3 spawnOffset = new Vector3(0f, 0.2f, 0f);

    private GameObject currentObject;

    public void SpawnObject(GameObject gameObject)
    {
        // Destroy the current object if it exists
        if (currentObject != null)
        {
            Destroy(currentObject);
        }

        transform.Find("PrintArea").GetComponent<PrintArea>().MoveObjectsInside();

        // Find the spawn position based on the parent's child "TableLight" position plus an offset
        Vector3 spawnPosition = transform.Find("TableLight").position + spawnOffset;

        currentObject = Instantiate(gameObject, spawnPosition, transform.rotation);

        Debug.Log("Syringe spawned at: " + spawnPosition);
    }

    public void DeregisterObject(GameObject gameObject)
    {
        if (currentObject != null && currentObject == gameObject)
        {
            currentObject = null;
        }
    }
}
