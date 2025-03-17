using UnityEngine;

public class SyringeTipBehaviour : MonoBehaviour
{
    NewSyringeBehaviour syringe;


    void Start(){
        syringe = GetComponentInParent<NewSyringeBehaviour>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Get the parent of the collider object
        Transform parentTransform = other.transform.parent;

        // If there is a parent, check the tag of the parent object
        if (parentTransform != null)
        {
            // Debugging: Check which tag we are comparing
            Debug.Log("Other tag: " + other.tag);
            Debug.Log("Parent tag: " + parentTransform.tag);

            if (parentTransform.CompareTag("AlienBlood"))
            {
                syringe.SetTipInContactWithAlienBlood(true);
                Debug.Log("SyringeTip entered trigger with AlienBlood");
            }
            else if (parentTransform.CompareTag("Alien"))
            {
                Debug.Log($"Transform: {other.transform}, Script: {other.GetComponentInParent<AlienBehaviour>()}");
                syringe.SetTipInContactWithAlien(true,other.GetComponentInParent<AlienBehaviour>());
                Debug.Log("SyringeTip entered trigger with Alien");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Get the parent of the collider object
        Transform parentTransform = other.transform.parent;

        // If there is a parent, check the tag of the parent object
        if (parentTransform != null)
        {
            if (parentTransform.CompareTag("AlienBlood"))
            {
                syringe.SetTipInContactWithAlienBlood(false);
                Debug.Log("SyringeTip exited trigger with AlienBlood");
            }
            else if (parentTransform.CompareTag("Alien"))
            {
                syringe.SetTipInContactWithAlien(false, null);
                Debug.Log("SyringeTip exited trigger with Alien");
            }
        }
    }

}
