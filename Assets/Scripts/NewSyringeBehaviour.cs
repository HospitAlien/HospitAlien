using UnityEngine;
using Oculus.Interaction.HandGrab;

public class NewSyringeBehaviour : MonoBehaviour, IHandGrabUseDelegate
{
    private bool fullLiquid;
    private BloodType currentBloodType; // Blood type enum
    private Animator animator;
    private bool tipInContactWithAlienBlood;
    private bool tipInContactWithAlien;
    public Renderer liquidRenderer; // Public reference to be assigned in the Inspector
    AlienBehaviour alien;

    // Materials for each blood type
    public Material greenBloodMaterial;
    public Material redBloodMaterial;
    public Material blueBloodMaterial;

    // Enum for blood type
    public enum BloodType
    {
        None,
        Red,
        Green,
        Blue
    }

    void Start()
    {
        fullLiquid = true;
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("No animator component found on syringe!");
        }

        if (liquidRenderer == null)
        {
            Debug.LogError("No Renderer assigned to the liquidRenderer field!");
        }

        tipInContactWithAlienBlood = false;
        tipInContactWithAlien = false;
        currentBloodType = BloodType.None; // Default to None
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
                // If the tip is in contact with AlienBlood, fill the syringe and set the material
                animator.Play("FillSyringeAnimation", 0, 0f);

                // Change the material based on the current blood type
                if (liquidRenderer != null)
                {
                    switch (currentBloodType)
                    {
                        case BloodType.Red:
                            liquidRenderer.material = redBloodMaterial;  // Directly assign the material
                            break;
                        case BloodType.Green:
                            liquidRenderer.material = greenBloodMaterial;
                            break;
                        case BloodType.Blue:
                            liquidRenderer.material = blueBloodMaterial;
                            break;
                        default:
                            Debug.LogWarning("Unknown blood type!");
                            break;
                    }
                }
                else
                {
                    Debug.LogError("liquidRenderer is null, cannot assign material!");
                }

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

    // Set the blood type (this is called from SyringeTipBehaviour)
    public void SetFullBloodType(BloodType bloodType)
    {
        currentBloodType = bloodType;
    }
}
