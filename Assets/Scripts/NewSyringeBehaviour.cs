using UnityEngine;
using Oculus.Interaction.HandGrab;

public class NewSyringeBehaviour : MonoBehaviour, IHandGrabUseDelegate
{
    private bool fullLiquid;
    private string currentBloodType;
    private Animator animator;
    private bool tipInContactWithAlienBlood;
    private int tipInContactWithAlien;
    public Renderer liquidRenderer; // Public reference to be assigned in the Inspector
    Alien alien;

    // Materials for each blood type
    public Material greenBloodMaterial;
    public Material redBloodMaterial;
    public Material blueBloodMaterial;



    void Start()
    {

        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("No animator component found on syringe!");
        }

        if (liquidRenderer == null)
        {
            Debug.LogError("No Renderer assigned to the liquidRenderer field!");
        }

        fullLiquid = false;
        currentBloodType = "";
        tipInContactWithAlienBlood = false;
        tipInContactWithAlien = 0;
    }

    public void SetTipInContactWithAlienBlood(bool value)
    {
        tipInContactWithAlienBlood = value;
        Debug.Log("Received SetTipInContactWithAlienBlood: " + value);
    }

    public void TipEnteredAlien(Alien newAlien)
    {
        alien = newAlien;
        tipInContactWithAlien += 1;
        Debug.Log($"LEVELS DEEP {tipInContactWithAlien}");
    }

    public void TipExitedAlien()
    {
        tipInContactWithAlien -= 1;
        if (tipInContactWithAlien == 0)
        {
            alien = null;
        }

        if (tipInContactWithAlien < 0)
        {
            tipInContactWithAlien = 0;
        }
        Debug.Log($"LEVELS DEEP {tipInContactWithAlien}");
    }

    // Called when the trigger is pressed
    public void BeginUse()
    {
        if (fullLiquid)
        {
            // If syringe is full, play SyringeAnimation and set fullLiquid to false
            animator.Play("SyringeAnimation", 0, 0f);
            // If the syringe tip is in contact with an Alien, send the "Syrined" message
            if (tipInContactWithAlien > 0)
            {
                alien.Syrined(currentBloodType);
                tipInContactWithAlien = 0;
            }

            fullLiquid = false;
            currentBloodType = "";
        }
        else
        {
            // If syringe is not full
            if (tipInContactWithAlienBlood)
            {
                // If the tip is in contact with AlienBlood, fill the syringe and set the material
                animator.Play("FillSyringeAnimation", 0, 0f);

                // Change the material based on the current blood type
                if (liquidRenderer != null)
                {
                    if (currentBloodType == "Red")
                    {
                        liquidRenderer.material = redBloodMaterial;  // Directly assign the material
                    }
                    else if (currentBloodType == "Blue")
                    {
                        liquidRenderer.material = blueBloodMaterial;
                    }
                    else
                    {
                        liquidRenderer.material = greenBloodMaterial;
                    }
                }
                else
                {
                    Debug.LogError("liquidRenderer is null, cannot assign material!");
                }

                fullLiquid = true;
            }
            else
            {
                // If the tip is not in contact with AlienBlood, play EmptySyringeAnimation
                animator.Play("EmptySyringeAnimation", 0, 0f);
                fullLiquid = false;
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

    // Set the blood type (this is called from SyringeTipBehaviour)
    public void SetFullBloodType(string bloodType)
    {
        currentBloodType = bloodType;
    }
}
