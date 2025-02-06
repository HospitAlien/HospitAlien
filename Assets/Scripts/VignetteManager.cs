using Unity.Mathematics;
using UnityEngine;


public class VignetteManager : MonoBehaviour
{
    public OVRVignette vignette;
    public OVRCameraRig cameraRig;
    public float VignetteStrength = 100;
    private const float VIGNETTE_OFF = 160;


    private Quaternion lastRotation;
    private float targetVignetteFieldOfView;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastRotation = cameraRig.transform.rotation;
        vignette.enabled = false;
        targetVignetteFieldOfView = VIGNETTE_OFF;
    }

    // Update is called once per frame
    void Update()
    {
        // get the rotation of the camera rig
        Quaternion currentRotation = cameraRig.transform.rotation;
        // if camera is rotating bigger than 10 degree per sec, turn on the vignette
        if (Quaternion.Angle(currentRotation, lastRotation) / Time.deltaTime > 10)
        {
            targetVignetteFieldOfView = 140 - VignetteStrength;
        }
        else targetVignetteFieldOfView = VIGNETTE_OFF;

        lastRotation = currentRotation;

        // smooth transition of the vignette
        vignette.VignetteFieldOfView = Mathf.Lerp(vignette.VignetteFieldOfView, targetVignetteFieldOfView, Time.deltaTime * 20.0f);

        if (vignette.VignetteFieldOfView < VIGNETTE_OFF)
        { vignette.enabled = true; }
        // when the vignette is off, disable the vignette component to save performance
        else vignette.enabled = false;
    }
}
