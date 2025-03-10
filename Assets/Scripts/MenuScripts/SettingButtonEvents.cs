using UnityEngine;

// this script should attached to game setting menu to let buttons work
public class SettingButtonEvents : MonoBehaviour
{
    private GlobalVariableManager gvm;

    void Start()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        if (gvm == null) Debug.LogError("GlobalVariableManager not found");
    }

    public void SetComfortModeToNone(bool isOn)
    {
        if (isOn) gvm.gameSettings.ComfortModeOption = GameSettingsIO.comfortMode.None;
        Debug.Log("Comfort mode set to None");
    }

    public void SetComfortModeToVignette(bool isOn)
    {
        if (isOn) gvm.gameSettings.ComfortModeOption = GameSettingsIO.comfortMode.Vignette;
        Debug.Log("Comfort mode set to Vignette");
    }

    public void SetComfortModeToPassthrough(bool isOn)
    {
        if (isOn) gvm.gameSettings.ComfortModeOption = GameSettingsIO.comfortMode.Passthrough;
    }

    public void SetMoveModeToTeleport(bool isOn)
    {
        if (isOn) gvm.gameSettings.MoveModeOption = GameSettingsIO.moveMode.teleport;
    }

    public void SetMoveModeToBoth(bool isOn)
    {
        if (isOn) gvm.gameSettings.MoveModeOption = GameSettingsIO.moveMode.both;
    }

    public void SetMoveModeToWalk(bool isOn)
    {
        if (isOn) gvm.gameSettings.MoveModeOption = GameSettingsIO.moveMode.walk;
    }

    public void SetVignetteStrength(float strength)
    {
        gvm.gameSettings.VignetteStrength = strength;
    }

    public void SetCameraHeight(float height)
    {
        gvm.gameSettings.CameraHeight = height / 100;
    }
}