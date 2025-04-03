using UnityEngine;


public class VignetteProvider : MonoBehaviour
{
    public OVRVignette vignette;
    public float VignetteStrength = 50;

    private const float VIGNETTE_OFF = 120;
    public float targetVignetteFieldOfView;


    void Awake()
    {
        targetVignetteFieldOfView = VIGNETTE_OFF;
    }

    public void UpdateVignetteStrength(float strength)
    {
        VignetteStrength = strength;
    }

    void Update()
    {
        // smooth transition of the vignette
        vignette.VignetteFieldOfView = Mathf.Lerp(vignette.VignetteFieldOfView, targetVignetteFieldOfView, Time.deltaTime * 20.0f);

        if (vignette.VignetteFieldOfView < VIGNETTE_OFF - 1.0)
        { vignette.enabled = true; }
        else vignette.enabled = false;

    }

    public void TurnVignetteOn()
    {
        targetVignetteFieldOfView = 90 - VignetteStrength;
    }

    public void TurnVignetteOff(bool immediate = false)
    {
        targetVignetteFieldOfView = VIGNETTE_OFF;
        if (immediate)
        {
            vignette.VignetteFieldOfView = VIGNETTE_OFF;
            vignette.enabled = false;
        }
    }
}
