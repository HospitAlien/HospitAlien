using UnityEngine;

// this script should attached to game setting menu to let buttons work
public class DetectExtinguisherGrab : MonoBehaviour
{
    public GameObject Tip1;
    public GameObject Tip2;

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Fire-Extinguisher"))
        {
            Tip1.SetActive(false);
            Tip2.SetActive(true);
        }
        Debug.Log(collider.name);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Fire-Extinguisher"))
        {
            Tip1.SetActive(true);
            Tip2.SetActive(false);
        }
    }
}