using System;
using UnityEngine;

// Create a new scriptable object to store global variables
[CreateAssetMenu(fileName = "settings", menuName = "Hospitalien/settings")]

public class GameSettingsIO : ScriptableObject
{
    [Header("Comfort Options")]
    [Tooltip("0 = vignette, 1 = passthrough(Not Completed), 2 = Off")]
    public int comfortMode = 0;

    [Header("Vignette Strength")]
    [Tooltip("Range 0.0 - 50.0")]
    public float vignetteStrength = 50.0f;

    public int ComfortMode
    {
        get => comfortMode;
        set
        {
            comfortMode = value;
            OnSettingChanged?.Invoke(this);
        }
    }

    public float VignetteStrength
    {
        get => vignetteStrength;
        set
        {
            vignetteStrength = value;
            OnSettingChanged?.Invoke(this);
        }
    }

    public event Action<GameSettingsIO> OnSettingChanged;
}
