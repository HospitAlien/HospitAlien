using UnityEngine;


public class ControllerManager : MonoBehaviour
{
    public OVRVignette vignette;
    private const float VIGNETTE_ON = 40;
    private const float VIGNETTE_OFF = 160;
    private float LStickValueX;
    private float RStickValueX;

    private float targetVignetteFieldOfView;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vignette.enabled = false;
        targetVignetteFieldOfView = VIGNETTE_OFF;
    }

    // Update is called once per frame
    void Update()
    {
        LStickValueX = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
        RStickValueX = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;
        // if camera is rotating, turn on the vignette
        if (Mathf.Abs(LStickValueX + RStickValueX) > 0.1f)
        {
            if (Mathf.Abs(LStickValueX) >= 0.5f || Mathf.Abs(RStickValueX) >= 0.5f)
            {
                targetVignetteFieldOfView = VIGNETTE_ON;
            }
            else targetVignetteFieldOfView = VIGNETTE_OFF;
        }
        else targetVignetteFieldOfView = VIGNETTE_OFF;

        // smooth transition of the vignette
        vignette.VignetteFieldOfView = Mathf.Lerp(vignette.VignetteFieldOfView, targetVignetteFieldOfView, Time.deltaTime * 20.0f);

        if (vignette.VignetteFieldOfView < VIGNETTE_OFF + 1.0f)
        { vignette.enabled = true; }
        // when the vignette is off, disable the vignette component to save performance
        else vignette.enabled = false;
    }
}
