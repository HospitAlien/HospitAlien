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
    public GameObject SetComfort;
    public GameObject SetMoveMode;
    public GameObject FinishTutorial;
    public GameObject FinishTips;
    public GameObject TouchTutorial;
    public GameObject DistanceGrabUseTutorial;
    private bool isFinished = false;

    void Start()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        mainCamera = Camera.main;
        mainCamera.cullingMask = LayerMask.GetMask("InitUI");
        moveManager.DisableMovement();
        SelectTutorial.SetActive(false);
        SetComfort.SetActive(false);
        SetMoveMode.SetActive(false);
        FinishTutorial.SetActive(false);
        TouchTutorial.SetActive(false);
        DistanceGrabUseTutorial.SetActive(false);
        gvm.gameSettings.ComfortModeOption = GameSettingsIO.comfortMode.Vignette;
        gvm.gameSettings.VignetteStrength = 50.0f;
        gvm.gameSettings.MoveModeOption = GameSettingsIO.moveMode.both;
    }

    void Update()
    {
        if (!isInitd)
        {
            MoveToPlayer(InitText); // Always show the tips in front of the player
            menuManager.CloseMenu(); // Make sure the menu is closed
            if (OVRInput.GetDown(OVRInput.Button.Two)) InitCamera();
        }
        if (isFinished)
        {
            MoveToPlayer(FinishTips, new Vector3(-0.5f, 0f, 0f));
        }
    }

    private void MoveToPlayer(GameObject obj, Vector3 offset = default)
    {
        Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * 1f;
        Vector3 playerRelativeOffset = mainCamera.transform.right * offset.x + mainCamera.transform.up * offset.y + mainCamera.transform.forward * offset.z;
        targetPosition += playerRelativeOffset;
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
        SetComfort.SetActive(true);
    }

    public void FinishSetComfort()
    {
        SetComfort.SetActive(false);
        SetMoveMode.SetActive(true);
        moveManager.EnableMovement();
    }

    public void FinishSetMoveMode()
    {
        SetMoveMode.SetActive(false);
        DistanceGrabUseTutorial.SetActive(true);
        menuManager.ToggleMenu(true);
        menuManager.RefreshMenu();
    }

    public void FinishDistanceGrabUseTutorial()
    {
        DistanceGrabUseTutorial.SetActive(false);
        TouchTutorial.SetActive(true);
    }

    public void Finish()
    {
        TouchTutorial.SetActive(false);
        FinishTutorial.SetActive(true);
        FinishTips.SetActive(true);
        isFinished = true;
    }
}
