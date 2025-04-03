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
        Transform parentTransform = other.transform.parent;
        if (parentTransform != null)
        {

            if (parentTransform.CompareTag("GreenAlienBlood"))
            {
                syringe.SetTipInContactWithAlienBlood(true);
                syringe.SetFullBloodType("Green"); // Set blood type to Green
            }
            else if (parentTransform.CompareTag("RedAlienBlood"))
            {
                syringe.SetTipInContactWithAlienBlood(true);
                syringe.SetFullBloodType("Red"); // Set blood type to Red
            }
            else if (parentTransform.CompareTag("BlueAlienBlood"))
            {
                syringe.SetTipInContactWithAlienBlood(true);
                syringe.SetFullBloodType("Blue"); // Set blood type to Blue
            }
        }

        Alien alien = other.GetComponentInParent<Alien>();
        if (alien != null)
        {
            syringe.TipEnteredAlien(alien);
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
        }

        Alien alien = other.GetComponentInParent<Alien>();
        if (alien != null)
        {
            syringe.TipExitedAlien();
            Debug.Log("SyringeTip exited trigger with Alien");
        }
    }
}
