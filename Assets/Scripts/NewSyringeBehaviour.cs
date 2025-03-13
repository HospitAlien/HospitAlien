using UnityEngine;
using System.Collections;
using Oculus.Interaction.HandGrab;

public class SyringeNewBehaviour : MonoBehaviour, IHandGrabUseDelegate
{
    private bool ready;
    private bool injecting; 
    private bool fullLiquid; 
    private Animation animationComponent; 

    void Start()
    {
        fullLiquid = true; 
        ready = true;
        injecting = false; 
        animationComponent = GetComponent<Animation>(); 

        if (animationComponent == null)
        {
            Debug.LogError("No Animation component found on syringe!");
        }
    }

    // Called when the trigger is pressed
    public void BeginUse()
    {
        if (fullLiquid && !injecting)
        {
            injecting = true;
            animationComponent.Play("Syringe Animation"); 

            StartCoroutine(WaitForInjection());
        }
    }

    private IEnumerator WaitForInjection()
    {
        yield return new WaitForSeconds(1f); // animation is 1 second long

        injecting = false; // Stop injecting after animation finishes
        fullLiquid = false; // empty
    }

    void OnCollisionEnter(Collision collision)
    {
        if (fullLiquid)
        {
            // Only interact with the alien if the syringe is full
            if (collision.gameObject.CompareTag("Alien"))
            {
                if (injecting)
                {
                    collision.gameObject.SendMessage("Syrined"); // Send message when injecting
                }
            }
        }
    }

      public float ComputeUseStrength(float strength)
    {
        Debug.Log("Compute use strength, strength: " + strength);
        return strength;
    }

    public void EndUse()
    {
        Debug.Log("End use");
       
    }

}