using UnityEngine;

public class PassthroughProvider : MonoBehaviour
{
    [Tooltip("Objects that shouldn't be rendered during passthrough")]
    [Header("Passthrough Objects To Remove")]
    [SerializeField] private GameObject[] _objects;

    public OVRPassthroughLayer layer;
    private Camera _camera;
    private bool _isPassThroughOn = false;
    private bool _KeepPassThroughOn = false;

    private void Awake()
    {
        _camera = OVRManager.FindMainCamera();
        Debug.Log("Camera: " + _camera);

        if (OVRManager.HasInsightPassthroughInitFailed())
        {
            enabled = false;
            Debug.LogError("Insight passthrough initialization failed.");
            return;
        }
        else
        {
            if (_isPassThroughOn) TurnPassThroughOn();
            else TurnPassThroughOff();
        }
    }

    public void TogglePassThrough(bool isOn)
    {
        if (isOn)
        {
            _KeepPassThroughOn = true;
            TurnPassThroughOn();
        }
        else
        {
            _KeepPassThroughOn = false;
            TurnPassThroughOff();
        }
    }

    public void TurnPassThroughOn()
    {
        _isPassThroughOn = true;
        _camera.clearFlags = CameraClearFlags.SolidColor;
        layer.textureOpacity = 1;
        layer.enabled = true;
        foreach (GameObject obj in _objects)
        {
            // Check if the renderer is not null before accessing it
            if (obj.GetComponent<Renderer>() != null)
            {
                obj.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    public void TurnPassThroughOff()
    {
        if (_KeepPassThroughOn) return;
        _isPassThroughOn = false;
        _camera.clearFlags = CameraClearFlags.Skybox;
        layer.textureOpacity = 0;
        layer.enabled = false;
        foreach (GameObject obj in _objects)
        {
            // Check if the renderer is not null before accessing it
            if (obj.GetComponent<Renderer>() != null)
            {
                obj.GetComponent<Renderer>().enabled = true;
            }
        }
    }
}
