using UnityEngine;

// Create a new scriptable object to store global variables
[CreateAssetMenu(fileName = "settings", menuName = "Hospitalien/settings")]

public class GameSettingsIO : ScriptableObject
{
    [Header("Comfort Options")]
    [Tooltip("0 = vignette, 1 = passthrough(Not Completed), 2 = Off")]
    public int comfortMode = 0;

    [Header("Vignette Strength")]
    [Tooltip("Range 30.0 - 100.0")]
    public float vignetteStrength = 100.0f;

}
