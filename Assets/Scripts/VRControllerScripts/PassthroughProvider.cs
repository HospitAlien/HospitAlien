using UnityEngine;

public class PassthroughProvider : MonoBehaviour
{
    [Tooltip("Objects that shouldn't be rendered during passthrough")]
    [Header("Passthrough Objects To Remove")]
    [SerializeField] private GameObject[] _objects;

    public OVRPassthroughLayer layer;
    private Camera _camera;
    private bool _isPassThroughOn = false;

    private void Start()
    {
        _camera = OVRManager.FindMainCamera();

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

    public void TogglePassThrough()
    {
        if (_isPassThroughOn) TurnPassThroughOff();
        else TurnPassThroughOn();
    }

    public void TurnPassThroughOn()
    {
        _isPassThroughOn = true;
        layer.textureOpacity = 1;
        _camera.clearFlags = CameraClearFlags.SolidColor;
        foreach (GameObject obj in _objects)
        {
            obj.GetComponent<Renderer>().enabled = false;
        }
    }

    public void TurnPassThroughOff()
    {
        _isPassThroughOn = false;
        layer.textureOpacity = 0;
        _camera.clearFlags = CameraClearFlags.Skybox;
        foreach (GameObject obj in _objects)
        {
            obj.GetComponent<Renderer>().enabled = true;
        }
    }
}
