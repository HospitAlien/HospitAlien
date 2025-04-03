using System;
using UnityEngine;

// Create a new scriptable object to store global variables
[CreateAssetMenu(fileName = "Settings", menuName = "Hospitalien/Settings")]

public class GameSettingsIO : ScriptableObject
{
    public enum comfortMode
    {
        Vignette,
        Passthrough,
        None
    }
    [Header("Comfort Options")]
    [SerializeField] private comfortMode comfortModeOption = comfortMode.Vignette;

    [Header("Vignette Strength")]
    [Tooltip("Range 0.0 - 50.0")]
    [SerializeField] private float vignetteStrength = 50.0f;

    public enum moveMode
    {
        teleport,
        both,
        walk
    }
    [Header("Movement Options")]
    [SerializeField] private moveMode moveModeOption = moveMode.teleport;

    [Header("Camera Height")]
    [Tooltip("Range 0.6m(2ft) - 2.4m(8ft), the base line is 1.8m(6ft)")]
    [SerializeField] private float cameraHeight = 1.8f;


    public comfortMode ComfortModeOption
    {
        get => comfortModeOption;
        set
        {
            comfortModeOption = value;
            OnComfortSettingChanged?.Invoke(this);
        }
    }

    public float VignetteStrength
    {
        get => vignetteStrength;
        set
        {
            vignetteStrength = value;
            OnComfortSettingChanged?.Invoke(this);
        }
    }

    public moveMode MoveModeOption
    {
        get => moveModeOption;
        set
        {
            moveModeOption = value;
            OnSettingChanged?.Invoke(this);
        }
    }

    public float CameraHeight
    {
        get => cameraHeight;
        set
        {
            cameraHeight = value;
            OnSettingChanged?.Invoke(this);
        }
    }

    public event Action<GameSettingsIO> OnSettingChanged;
    public event Action<GameSettingsIO> OnComfortSettingChanged;
}