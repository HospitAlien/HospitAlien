using UnityEngine;


// Attach this to tracking space to manage the height of the camera rig
public class CameraHeightManager : MonoBehaviour
{
    private GlobalVariableManager gvm;
    private Camera _mainCamera;
    private float cameraHeightBaseline = 1.82f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        _mainCamera = OVRManager.FindMainCamera();
        gvm.gameSettings.OnSettingChanged += OnCameraHeightChanged;
    }

    private void OnCameraHeightChanged(GameSettingsIO settings)
    {
        transform.position = new Vector3(transform.position.x, cameraHeightBaseline - settings.CameraHeight, transform.position.z);
    }

    public void SetCameraHeightBaseline(float baseLine)
    {
        cameraHeightBaseline = baseLine;
        transform.position = new Vector3(transform.position.x, cameraHeightBaseline - gvm.gameSettings.CameraHeight, transform.position.z);
    }

    public void ResetCameraHeight()
    {
        gvm.gameSettings.CameraHeight = _mainCamera.transform.position.y - transform.position.y;
    }
}
