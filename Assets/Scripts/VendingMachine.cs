using System;
using System.Collections;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    // Offset relative to the parent (vending machine) position
    public Vector3 spawnOffset = new Vector3(0f, 0.2f, 0f);

    private GameObject currentObject;
    private bool isCoolingDown = false;

    public void SpawnObject(GameObject gameObject)
    {
        if (isCoolingDown)
        {
            return;
        }
        // Destroy the current object if it exists
        if (currentObject != null)
        {
            Destroy(currentObject);
        }

        transform.Find("PrintArea").GetComponent<PrintArea>().MoveObjectsInside();

        // Find the spawn position based on the parent's child "TableLight" position plus an offset
        Vector3 spawnPosition = transform.Find("TableLight").position + spawnOffset;

        currentObject = Instantiate(gameObject, spawnPosition, transform.rotation);

        // Cool down before spawning another object
        StartCoroutine(CoolDown());
    }

    IEnumerator CoolDown()
    {
        OnCoolDown?.Invoke(true);
        isCoolingDown = true;
        yield return new WaitForSeconds(1f);
        isCoolingDown = false;
        OnCoolDown?.Invoke(false);
    }

    public void DeregisterObject(GameObject gameObject)
    {
        if (currentObject != null && currentObject == gameObject)
        {
            currentObject = null;
        }
    }

    public event Action<bool> OnCoolDown;
}
