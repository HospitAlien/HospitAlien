using UnityEngine;

public class VendingButton : MonoBehaviour
{
    public GameObject syringePrefab;

    // Offset relative to the parent (vending machine) position
    public Vector3 spawnOffset = new Vector3(0f, 0.1f, 0f);

    void Start() { }

    private void OnTriggerEnter()
    {
        SpawnSyringe();
    }


    void SpawnSyringe()
    {
        Vector3 spawnPosition = transform.parent.Find("TableLight").position + spawnOffset;

        Instantiate(syringePrefab, spawnPosition, transform.parent.rotation);
        Debug.Log("Syringe spawned at: " + spawnPosition);
    }
}
