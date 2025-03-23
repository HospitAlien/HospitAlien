using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject InitText;
    private Camera mainCamera;
    private GlobalVariableManager gvm;
    public MoveManager moveManager;
    public MenuManager menuManager;
    private bool isInitd = false;
    public GameObject SelectTutorial;

    void Start()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        mainCamera = Camera.main;
        mainCamera.cullingMask = LayerMask.GetMask("InitUI");
        moveManager.DisableMovement();
        SelectTutorial.SetActive(false);
    }

    void Update()
    {
        if (!isInitd)
        {
            MoveToPlayer(InitText); // Always show the tips in front of the player
            menuManager.CloseMenu(); // Make sure the menu is closed
            if (OVRInput.GetDown(OVRInput.Button.Two)) InitCamera();
        }
    }

    private void MoveToPlayer(GameObject obj, Vector3 offset = default)
    {
        Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * 1f;
        targetPosition += offset;
        obj.transform.position = targetPosition;
        Vector3 directionToCamera = mainCamera.transform.position - obj.transform.position;
        obj.transform.rotation = Quaternion.LookRotation(-directionToCamera);
    }

    private void InitCamera()
    {
        FindFirstObjectByType<CameraHeightManager>().ResetCameraHeight();
        mainCamera.cullingMask = -1;
        isInitd = true;
        InitText.SetActive(false);
        SelectTutorial.SetActive(true);
    }

    public void FinishSelectTutorial()
    {
        SelectTutorial.SetActive(false);
    }
}
