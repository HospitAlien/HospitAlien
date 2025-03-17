using UnityEngine;
using Oculus.Interaction.HandGrab;

public class NewSyringeBehaviour : MonoBehaviour, IHandGrabUseDelegate
{
    private bool fullLiquid; 
    private Animator animator;
    private bool tipInContactWithAlienBlood;
    private bool tipInContactWithAlien;
    AlienBehaviour alien;

    void Start()
    {
        fullLiquid = true;
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("No animator component found on syringe!");
        }

        tipInContactWithAlienBlood = false;
        tipInContactWithAlien = false;
    }

    public void SetTipInContactWithAlienBlood(bool value)
    {
        tipInContactWithAlienBlood = value;
        Debug.Log("Received SetTipInContactWithAlienBlood: " + value);
    }

    public void SetTipInContactWithAlien(bool value, AlienBehaviour newAlien)
    {
        alien = newAlien;
        tipInContactWithAlien = value;
        Debug.Log("Received SetTipInContactWithAlien: " + value);
        Debug.Log(newAlien);
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
                alien.SendMessage("Syrined");
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
    }

    public void EndUse()
    {
    }

    public float ComputeUseStrength(float strength)
    {
         return strength;
    }
}
