using TMPro;
using UnityEngine;
using UnityEngine.UI;

// this script should attached to game setting menu to let buttons work
public class TutorialMoveModeButton : MonoBehaviour
{
    private GlobalVariableManager gvm;
    public Transform CameraRig;

    public Toggle TeleportButton;
    public Toggle BothButton;
    public Toggle WalkButton;
    public TextMeshProUGUI TeleportButtonText;
    public TextMeshProUGUI BothButtonText;
    public TextMeshProUGUI WalkButtonText;

    public GameObject TeleportImage;
    public GameObject BothImage;
    public GameObject WalkImage;

    void Awake()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        if (gvm == null) Debug.LogError("GlobalVariableManager not found");
        gvm.gameSettings.OnSettingChanged += OnSettingChanged;
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two)) MovePlayerBack();
    }

    private void MovePlayerBack()
    {
        CameraRig.position = new Vector3(1.2f, 0, -0.66f);
    }

    public void SetMoveModeToTeleport()
    {
        gvm.gameSettings.MoveModeOption = GameSettingsIO.moveMode.teleport;
        SetButtonsStatus(0);
    }

    public void SetMoveModeToBoth()
    {
        gvm.gameSettings.MoveModeOption = GameSettingsIO.moveMode.both;
        SetButtonsStatus(1);
    }

    public void SetMoveModeToWalk()
    {
        gvm.gameSettings.MoveModeOption = GameSettingsIO.moveMode.walk;
        SetButtonsStatus(2);
    }


    private void SetButtonsStatus(int ButtonIndex)
    {
        TeleportButton.interactable = true;
        BothButton.interactable = true;
        WalkButton.interactable = true;
        TeleportButtonText.text = "Select";
        BothButtonText.text = "Select";
        WalkButtonText.text = "Select";
        TeleportImage.SetActive(false);
        BothImage.SetActive(false);
        WalkImage.SetActive(false);
        switch (ButtonIndex)
        {
            case 0:
                TeleportButton.interactable = false;
                TeleportButtonText.text = "Selected";
                TeleportImage.SetActive(true);
                break;
            case 1:
                BothButton.interactable = false;
                BothButtonText.text = "Selected";
                BothImage.SetActive(true);
                break;
            case 2:
                WalkButton.interactable = false;
                WalkButtonText.text = "Selected";
                WalkImage.SetActive(true);
                break;
        }
    }

    private void OnSettingChanged(GameSettingsIO gameSettings)
    {
        switch (gameSettings.MoveModeOption)
        {
            case GameSettingsIO.moveMode.teleport:
                SetButtonsStatus(0);
                break;
            case GameSettingsIO.moveMode.both:
                SetButtonsStatus(1);
                break;
            case GameSettingsIO.moveMode.walk:
                SetButtonsStatus(2);
                break;
        }
    }
}