using UnityEngine;


public class ComfortManager : MonoBehaviour
{
    public VignetteProvider vignetteProvider;
    public OVRCameraRig cameraRig;
    // Enable passthrough if there is one provider in scene
    private PassthroughProvider _passthroughProvider;
    private float remainTime;


    private Quaternion lastRotation;
    private Vector3 lastPosition;
    private GlobalVariableManager gvm;
    private GameSettingsIO.comfortMode comfortMode;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        gvm.gameSettings.OnComfortSettingChanged += OnComfortSettingChanged;
        _passthroughProvider = FindFirstObjectByType<PassthroughProvider>();
    }

    private void OnComfortSettingChanged(GameSettingsIO settings)
    {
        if (comfortMode != settings.ComfortModeOption) TurnOffComfortMode(true);
        comfortMode = settings.ComfortModeOption;
        if (comfortMode == GameSettingsIO.comfortMode.Vignette)
        {
            vignetteProvider.enabled = true;
            _passthroughProvider.enabled = false;
            vignetteProvider.UpdateVignetteStrength(settings.VignetteStrength);
        }
        else if (comfortMode == GameSettingsIO.comfortMode.Passthrough)
        {
            vignetteProvider.enabled = false;
            _passthroughProvider.enabled = true;
        }
        else
        {
            vignetteProvider.enabled = false;
            _passthroughProvider.enabled = false;
        }
        ActiveComfortMode(1f);
    }

    // Update is called once per frame
    void Update()
    {
        // do nothing if the comfort mode is none
        if (comfortMode == GameSettingsIO.comfortMode.None) return;

        if (gvm.gameSettings.TurnModeOption == GameSettingsIO.turnMode.Smooth)
        {
            // get the rotation of the camera rig
            Quaternion currentRotation = cameraRig.transform.rotation;
            // if camera is rotating bigger than 5 degree per sec, turn on the vignette
            if (Quaternion.Angle(currentRotation, lastRotation) / Time.deltaTime > 5)
            {
                ActiveComfortMode(0.05f);
            }
            lastRotation = currentRotation;
        }

        if (gvm.gameSettings.MoveModeOption != GameSettingsIO.moveMode.teleport)
        {
            // get the position of the camera rig
            Vector3 currentPosition = cameraRig.transform.position;
            // if camera is smoothly moving, turn on the vignette
            float speed = Vector3.Distance(currentPosition, lastPosition) / Time.deltaTime;
            if (speed > 0.1f && speed < 7)
            {
                ActiveComfortMode(0.05f);
            }
            lastPosition = currentPosition;
        }

        // when the time is up, turn off the vignette
        if (remainTime < 0f)
        {
            TurnOffComfortMode();
        }
        else
            remainTime -= Time.deltaTime;
    }

    private void TurnOnComfortMode()
    {
        if (comfortMode == GameSettingsIO.comfortMode.Vignette)
            vignetteProvider.TurnVignetteOn();
        else if (comfortMode == GameSettingsIO.comfortMode.Passthrough)
            _passthroughProvider.TurnPassThroughOn();
    }

    private void TurnOffComfortMode(bool immediate = false)
    {
        if (comfortMode == GameSettingsIO.comfortMode.Vignette)
            vignetteProvider.TurnVignetteOff(immediate);
        else if (comfortMode == GameSettingsIO.comfortMode.Passthrough)
            _passthroughProvider.TurnPassThroughOff();
    }

    public void ActiveComfortMode(float seconds)
    {
        TurnOnComfortMode();
        remainTime = seconds;
    }

    void OnDestroy()
    {
        gvm.gameSettings.OnComfortSettingChanged -= OnComfortSettingChanged;
    }
}
