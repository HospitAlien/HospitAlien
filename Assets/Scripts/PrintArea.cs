using UnityEngine;
using System.Collections.Generic;

public class PrintArea : MonoBehaviour
{
    private List<GameObject> objectsInside = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != gameObject)
        {
            objectsInside.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Object exited print area" + other.gameObject.name);
        transform.parent.GetComponent<VendingMachine>().DeregisterObject(other.gameObject);
        objectsInside.Remove(other.gameObject);
    }

    public void MoveObjectsInside()
    {
        foreach (GameObject obj in objectsInside)
        {
            if (obj != null)
            {
                obj.transform.Translate(new Vector3(-3f, 1f, 0f), Space.World);
            }
        }
    }
}
