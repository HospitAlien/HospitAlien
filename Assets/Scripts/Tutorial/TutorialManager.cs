using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject InitText;
    private Camera mainCamera;
    private GlobalVariableManager gvm;
    private bool isInitd = false;

    void Start()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        mainCamera = Camera.main;
        mainCamera.cullingMask = LayerMask.GetMask("InitUI");
    }

    void Update()
    {
        if (!isInitd)
        {
            MoveToPlayer(InitText);
        }
        if (OVRInput.GetDown(OVRInput.Button.Two)) InitCamera();
    }

    private void InitCamera()
    {
        mainCamera.cullingMask = -1;
        isInitd = true;
        InitText.SetActive(false);
    }


    public void MoveToPlayer(GameObject obj, Vector3 offset = default)
    {
        Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * 1f;
        targetPosition += offset;
        obj.transform.position = targetPosition;
        obj.transform.LookAt(mainCamera.transform.position);
    }
}
