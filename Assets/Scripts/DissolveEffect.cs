using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    [Header("Dissolve Settings")]
    [Tooltip("How many seconds it takes for the object to fully dissolve in.")]
    [SerializeField] private float dissolveDuration = 2f;

    [Tooltip("Start value for _CutoffHeight (object is invisible at start).")]
    [SerializeField] private float startCutoff = 2f;

    [Tooltip("End value for _CutoffHeight (object is visible at end).")]
    [SerializeField] private float endCutoff = -2f;

    [Header("Shader Graph Reference")]
    [Tooltip("Drag the compiled shader from your Shader Graph here.")]
    [SerializeField] private Shader dissolveShader;  // <-- Assign in the Inspector
   
    // We'll store references to each new cloned material so we can animate them.
    private Material[] clonedMaterials;

    private float elapsedTime;

    private void Awake()
    {
        // 1) Get all MeshRenderers in children (and parent).
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        // Prepare array to store new materials
        clonedMaterials = new Material[renderers.Length];

        // 2) For each MeshRenderer, clone its material and switch to the assigned dissolve shader
        for (int i = 0; i < renderers.Length; i++)
        {
            // Get a unique instance of the material the renderer currently has
            Material originalMatInstance = renderers[i].material;
           
            // Create a new material from that instance (copies textures/colors)
            Material newDissolveMat = new Material(originalMatInstance);

            // If we have a valid shader, assign it; otherwise, log an error
            if (dissolveShader != null)
            {
                newDissolveMat.shader = dissolveShader;
            }
            else
            {
                Debug.LogError("No dissolve shader assigned in the Inspector! " +
                               "Please assign your Shader Graph's compiled shader.");
            }

            // Assign the new material back to the renderer
            renderers[i].material = newDissolveMat;

            // Initialize _CutoffHeight so it starts invisible (assuming that's what your shader does)
            newDissolveMat.SetFloat("_CutoffHeight", startCutoff);

            // Store in our array for later updates
            clonedMaterials[i] = newDissolveMat;
        }
    }

    private void Update()
    {
        // 3) Animate over time until the dissolve is finished
        if (elapsedTime < dissolveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / dissolveDuration);

            // Lerp from start to end
            float currentCutoff = Mathf.Lerp(startCutoff, endCutoff, t);

            // Apply to all cloned materials
            for (int i = 0; i < clonedMaterials.Length; i++)
            {
                if (clonedMaterials[i] != null)
                    clonedMaterials[i].SetFloat("_CutoffHeight", currentCutoff);
            }

            // Once we pass the duration, finalize and optionally disable
            if (elapsedTime >= dissolveDuration)
            {
                for (int i = 0; i < clonedMaterials.Length; i++)
                {
                    if (clonedMaterials[i] != null)
                        clonedMaterials[i].SetFloat("_CutoffHeight", endCutoff);
                }

                enabled = false;
            }
        }
    }
}
