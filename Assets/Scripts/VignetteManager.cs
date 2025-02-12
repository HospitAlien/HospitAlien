using UnityEngine;


public class VignetteManager : MonoBehaviour
{
    public OVRVignette vignette;
    public OVRCameraRig cameraRig;
    public float VignetteStrength = 50;
    private const float VIGNETTE_OFF = 120;
    private float remainTime;


    private Quaternion lastRotation;
    private float targetVignetteFieldOfView;

    private GlobalVariableManager gvm;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        remainTime = 0;
        lastRotation = cameraRig.transform.rotation;
        vignette.enabled = false;
        targetVignetteFieldOfView = VIGNETTE_OFF;
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        UpdateVignetteStrength(gvm.gameSettings);
        gvm.gameSettings.OnSettingChanged += UpdateVignetteStrength;
    }

    private void UpdateVignetteStrength(GameSettingsIO settings)
    {
        VignetteStrength = settings.vignetteStrength;
        ActiveVignette(500);
    }

    // Update is called once per frame
    void Update()
    {
        // get the rotation of the camera rig
        Quaternion currentRotation = cameraRig.transform.rotation;
        // if camera is rotating bigger than 10 degree per sec, turn on the vignette
        if (Quaternion.Angle(currentRotation, lastRotation) / Time.deltaTime > 10)
        {
            ActiveVignette(50);
        }
        lastRotation = currentRotation;

        // when the time is up, turn off the vignette
        if (remainTime > 0)
            remainTime -= Time.deltaTime * 1000;
        else
        {
            targetVignetteFieldOfView = VIGNETTE_OFF;
        }

        // smooth transition of the vignette
        vignette.VignetteFieldOfView = Mathf.Lerp(vignette.VignetteFieldOfView, targetVignetteFieldOfView, Time.deltaTime * 20.0f);

        if (vignette.VignetteFieldOfView < VIGNETTE_OFF - 1.0)
        { vignette.enabled = true; }
        // when the vignette is off, disable the vignette component to save performance
        else vignette.enabled = false;
    }

    private void ActiveVignette(float milliseconds)
    {
        vignette.enabled = true;
        targetVignetteFieldOfView = 90 - VignetteStrength;
        remainTime = milliseconds;
    }
}
