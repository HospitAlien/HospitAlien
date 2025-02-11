using UnityEngine;
using System.Collections;

public class SyrineBehaviour : MonoBehaviour
{
    private bool ready;
    public Material inactiveMaterial;
    private Material[] originalMaterials;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalMaterials = new Material[transform.childCount];
        int i = 0;
        foreach (Transform child in transform)
        {
            Renderer childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                // Store the original material if it exists, else store null
                originalMaterials[i] = childRenderer.sharedMaterial; // sharedMaterial refers to the material used by the renderer, it won't create a copy.
                i++;
            }
        }


        ready = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(ready){
            //We only want to interact with the alien if the status of the syringe is ready
            if(collision.gameObject.CompareTag("Alien")){
                collision.gameObject.SendMessage("Syrined");
                ready = false;
                updateMaterial();
                StartCoroutine(SyringeCooldown());
            }
        }

    }


    void updateMaterial(){

        if(!ready){
            foreach (Transform child in transform)
            {
                // Get the Renderer component of the child
                Renderer childRenderer = child.GetComponent<Renderer>();

                // If the child has a Renderer, change its material
                if (childRenderer != null)
                {
                    childRenderer.material = inactiveMaterial;
                }
            }
        }else{
            int i = 0;
            foreach (Transform child in transform)
            {
                Renderer childRenderer = child.GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    // Store the original material if it exists, else store null
                    childRenderer.material = originalMaterials[i];
                    i++;
                }
            }
        }

    }

    private IEnumerator SyringeCooldown()
    {
        yield return new WaitForSeconds(10f);
        ready = true;
        updateMaterial();

    }

}
