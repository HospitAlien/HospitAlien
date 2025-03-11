using UnityEngine;

public class VendingButton : MonoBehaviour
{
    public GameObject objectPrefab;

    private void OnTriggerEnter(Collider other)
    {
        transform.parent.GetComponent<VendingMachine>().SpawnObject(objectPrefab);
    }
}
