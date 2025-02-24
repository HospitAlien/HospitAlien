using UnityEngine;


public class VignetteManager : MonoBehaviour
{
    public OVRVignette vignette;
    private OVRCameraRig _cameraRig;
    public float VignetteStrength = 50;
    private const float VIGNETTE_OFF = 120;
    public float remainTime;


    private Quaternion lastRotation;
    private Vector3 lastPosition;

    private float targetVignetteFieldOfView;
    private GlobalVariableManager gvm;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        remainTime = 0;
        _cameraRig = FindFirstObjectByType<OVRCameraRig>();
        lastRotation = _cameraRig.transform.rotation;
        vignette.enabled = false;
        targetVignetteFieldOfView = VIGNETTE_OFF;
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        VignetteStrength = gvm.gameSettings.VignetteStrength;
        gvm.gameSettings.OnSettingChanged += UpdateVignetteStrength;
    }

    private void UpdateVignetteStrength(GameSettingsIO settings)
    {
        if (settings.ComfortModeOption != GameSettingsIO.comfortMode.Vignette)
        {
            VignetteStrength = 0;
            vignette.enabled = false;
        }
        else
        if (settings.VignetteStrength != VignetteStrength)
        {
            VignetteStrength = settings.VignetteStrength;
            ActiveVignette(1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gvm.gameSettings.ComfortModeOption != GameSettingsIO.comfortMode.Vignette)
        {
            return;
        }
        if (gvm.gameSettings.turnModeOption == GameSettingsIO.turnMode.Smooth)
        {
            // get the rotation of the camera rig
            Quaternion currentRotation = _cameraRig.transform.rotation;
            // if camera is rotating bigger than 5 degree per sec, turn on the vignette
            if (Quaternion.Angle(currentRotation, lastRotation) / Time.deltaTime > 5)
            {
                ActiveVignette(0.05f);
            }
            lastRotation = currentRotation;
        }

        if (gvm.gameSettings.moveModeOption != GameSettingsIO.moveMode.teleport)
        {
            // get the position of the camera rig
            Vector3 currentPosition = _cameraRig.transform.position;
            // if camera is smoothly moving, turn on the vignette
            float speed = Vector3.Distance(currentPosition, lastPosition) / Time.deltaTime;
            if (speed > 0.1 && speed < 2.5)
            {
                ActiveVignette(0.05f);
            }
            lastPosition = currentPosition;
        }

        // when the time is up, turn off the vignette
        if (remainTime < 0f)
            targetVignetteFieldOfView = VIGNETTE_OFF;
        else
            remainTime -= Time.deltaTime;

        // smooth transition of the vignette
        vignette.VignetteFieldOfView = Mathf.Lerp(vignette.VignetteFieldOfView, targetVignetteFieldOfView, Time.deltaTime * 20.0f);

        if (vignette.VignetteFieldOfView < VIGNETTE_OFF - 1.0)
        { vignette.enabled = true; }
        // when the vignette is off, disable the vignette component to save performance
        else vignette.enabled = false;

    }

    public void ActiveVignette(float seconds)
    {
        targetVignetteFieldOfView = 90 - VignetteStrength;
        remainTime = seconds;
    }
}
