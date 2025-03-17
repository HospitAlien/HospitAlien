using UnityEngine;
using System.Collections;
using Oculus.Interaction.HandGrab;

public class SyringeNewBehaviour : MonoBehaviour, IHandGrabUseDelegate
{
    private bool injecting; 
    private bool fullLiquid; 
    private Animator animator; 
    private bool tipInContactWithAlienBlood; // To track if the syringe tip is in contact with AlienBlood
    private bool tipInContactWithAlien;     // To track if the syringe tip is in contact with Alien

    void Start()
    {
        fullLiquid = true; 
        injecting = false; 
        animator = GetComponent<Animator>(); 

        if (animator == null)
        {
            Debug.LogError("No animator component found on syringe!");
        }

        tipInContactWithAlienBlood = false; 
        tipInContactWithAlien = false; 
    }

    // Called when the trigger is pressed
    public void BeginUse()
    {
        if (fullLiquid)
        {
            // If syringe is full, play SyringeAnimation and set fullLiquid to false
            animator.Play("SyringeAnimation", 0, 0f);
            fullLiquid = false;

            // If the syringe tip is in contact with an Alien, send the "Syrined" message
            if (tipInContactWithAlien)
            {
                gameObject.SendMessage("Syrined");
                Debug.Log("Syringe injected Alien, message 'Syrined' sent.");
            }
        }
        else
        {
            // If syringe is not full
            if (tipInContactWithAlienBlood)
            {
                // If the tip is in contact with AlienBlood, fill the syringe
                animator.Play("FillSyringeAnimation", 0, 0f);
                fullLiquid = true;
                Debug.Log("Syringe filled with AlienBlood, fullLiquid = true");
            }
            else
            {
                // If the tip is not in contact with AlienBlood, play EmptySyringeAnimation
                animator.Play("EmptySyringeAnimation", 0, 0f);
                fullLiquid = false;
                Debug.Log("Syringe is empty, fullLiquid = false");
            }
        }

        // Reset the injection flag after the use
        injecting = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collider is the SyringeTip
        if (other.CompareTag("SyringeTip"))
        {
            if (other.CompareTag("AlienBlood"))
            {
                tipInContactWithAlienBlood = true; // Set flag if in contact with AlienBlood
                Debug.Log("SyringeTip entered trigger with AlienBlood");
            }
            else if (other.CompareTag("Alien"))
            {
                tipInContactWithAlien = true; // Set flag if in contact with Alien
                Debug.Log("SyringeTip entered trigger with Alien");
            }
        }
    }

    // This checks when the trigger exit occurs, to reset the flags
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SyringeTip"))
        {
            if (other.CompareTag("AlienBlood"))
            {
                tipInContactWithAlienBlood = false; // Reset the flag when no longer in contact
                Debug.Log("SyringeTip exited trigger with AlienBlood");
            }
            else if (other.CompareTag("Alien"))
            {
                tipInContactWithAlien = false; // Reset the flag when no longer in contact
                Debug.Log("SyringeTip exited trigger with Alien");
            }
        }
    }

    public float ComputeUseStrength(float strength)
    {
        // Debug.Log("Compute use strength, strength: " + strength);
        return strength;
    }

    public void EndUse()
    {
        Debug.Log("End use");
    }
}
