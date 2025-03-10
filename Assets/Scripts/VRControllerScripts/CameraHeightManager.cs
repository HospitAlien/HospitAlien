using UnityEngine;


// Attach this to tracking space to manage the height of the camera rig
public class CameraHeightManager : MonoBehaviour
{
    private GlobalVariableManager gvm;
    private float cameraHeightOffset = -1.8f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        gvm.gameSettings.OnSettingChanged += OnCameraHeightChanged;
    }

    private void OnCameraHeightChanged(GameSettingsIO settings)
    {
        transform.position = new Vector3(transform.position.x, settings.CameraHeight + cameraHeightOffset, transform.position.z);
    }

    public void SetCameraHeightOffset(float offset)
    {
        cameraHeightOffset = offset;
        transform.position = new Vector3(transform.position.x, gvm.gameSettings.CameraHeight + cameraHeightOffset, transform.position.z);
    }
}
