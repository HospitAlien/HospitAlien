using UnityEngine;

public class DissolveEffect : MonoBehaviour
{

    [SerializeField] private float dissolveDuration = 3f;

    [SerializeField] private float startCutoff = -2f;

    [SerializeField] private float endCutoff = 2f;

    [SerializeField] private Shader dissolveShader;
   
    // store references to each new cloned material so we can animate them.
    private Material[] clonedMaterials;

    private float elapsedTime;

    private void Awake()
    {
        // get all MeshRenderers in children (and parent).
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        // Prepare array to store new materials
        clonedMaterials = new Material[renderers.Length];

        // for each MeshRenderer, clone its material and switch to the assigned dissolve shader
        for (int i = 0; i < renderers.Length; i++)
        {
            // Get a unique instance of the material the renderer currently has
            Material originalMatInstance = renderers[i].material;
           
            // Create a new material from that instance (copies textures/colors)
            Material newDissolveMat = new Material(originalMatInstance);

            // If we have a valid shader then assign it
            if (dissolveShader != null)
            {
                newDissolveMat.shader = dissolveShader;
            }
            else
            {
                Debug.Log("No dissolve shader assigned!");
            }

            // Copy the _BaseMap texture if available.
            if (originalMatInstance.HasProperty("_BaseMap") && newDissolveMat.HasProperty("_BaseMap"))
            {
                Texture originalTexture = originalMatInstance.GetTexture("_BaseMap");
                newDissolveMat.SetTexture("_BaseMap", originalTexture);
            }
            // Copy the _BaseColor if available.
            if (originalMatInstance.HasProperty("_BaseColor") && newDissolveMat.HasProperty("_BaseColor"))
            {
                Color originalColor = originalMatInstance.GetColor("_BaseColor");
                newDissolveMat.SetColor("_BaseColor", originalColor);
            }

            // assign the new material back to the renderer
            renderers[i].material = newDissolveMat;

            // Initialise _CutoffHeight so it starts invisible
            newDissolveMat.SetFloat("_CutoffHeight", startCutoff);

            // Store for later updates
            clonedMaterials[i] = newDissolveMat;
        }
    }

    private void Update()
    {
        // animate over time until the dissolve is finished
        if (elapsedTime < dissolveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / dissolveDuration);

            // Lerp from start to end - GPT thingamajig
            float currentCutoff = Mathf.Lerp(startCutoff, endCutoff, t);

            // Apply to all cloned materials
            for (int i = 0; i < clonedMaterials.Length; i++)
            {
                if (clonedMaterials[i] != null)
                    clonedMaterials[i].SetFloat("_CutoffHeight", currentCutoff);
            }

            // Once we pass the duration, finalise and disable
            if (elapsedTime >= dissolveDuration)
            {
                for (int i = 0; i < clonedMaterials.Length; i++)
                {
                    if (clonedMaterials[i] != null)
                        clonedMaterials[i].SetFloat("_CutoffHeight", endCutoff);
                }

                enabled = false; //script stops
            }
        }
    }
}