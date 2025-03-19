using UnityEngine;

public class SyringeTipBehaviour : MonoBehaviour
{
    private NewSyringeBehaviour syringe;

    void Start()
    {
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

            if (parentTransform.CompareTag("GreenAlienBlood"))
            {
                syringe.SetTipInContactWithAlienBlood(true);
                syringe.SetFullBloodType(NewSyringeBehaviour.BloodType.Green); // Set blood type to Green
                Debug.Log("SyringeTip entered trigger with GreenAlienBlood");
            }
            else if (parentTransform.CompareTag("RedAlienBlood"))
            {
                syringe.SetTipInContactWithAlienBlood(true);
                syringe.SetFullBloodType(NewSyringeBehaviour.BloodType.Red); // Set blood type to Red
                Debug.Log("SyringeTip entered trigger with RedAlienBlood");
            }
            else if (parentTransform.CompareTag("BlueAlienBlood"))
            {
                syringe.SetTipInContactWithAlienBlood(true);
                syringe.SetFullBloodType(NewSyringeBehaviour.BloodType.Blue); // Set blood type to Blue
                Debug.Log("SyringeTip entered trigger with BlueAlienBlood");
            }
            else if (parentTransform.CompareTag("Alien"))
            {
                syringe.SetTipInContactWithAlien(true, other.GetComponentInParent<AlienBehaviour>());
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
            if (parentTransform.CompareTag("GreenAlienBlood") || parentTransform.CompareTag("RedAlienBlood") || parentTransform.CompareTag("BlueAlienBlood"))
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
