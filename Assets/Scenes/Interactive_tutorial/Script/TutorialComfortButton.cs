using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// this script should attached to game setting menu to let buttons work
public class TutorialComfortButton : MonoBehaviour
{
    private GlobalVariableManager gvm;
    public Transform CameraRig;
    public GameObject TryButton;

    public Toggle VignetteButton;
    public Toggle PassthroughButton;
    public Toggle NoneButton;
    public TextMeshProUGUI VignetteButtonText;
    public TextMeshProUGUI PassthroughButtonText;
    public TextMeshProUGUI NoneButtonText;
    public Slider VignetteStrengthSlider;
    public TextMeshProUGUI VignetteStrengthText;
    public GameObject VignetteStrengthPanel;

    private string strengthText = "Vignette Strength: %0";

    void Awake()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        if (gvm == null) Debug.LogError("GlobalVariableManager not found");
        gvm.gameSettings.OnComfortSettingChanged += OnComfortSettingChanged;
    }

    public void HaveATry()
    {
        StartCoroutine(RotateSequenceCoroutine());
    }

    private IEnumerator RotateSequenceCoroutine()
    {
        TryButton.SetActive(false);
        float elapsedTime = 0f;
        Quaternion startRotation = CameraRig.transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 90, 0);

        while (elapsedTime < 1f)
        {
            CameraRig.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        CameraRig.transform.rotation = targetRotation;

        elapsedTime = 0f;
        startRotation = CameraRig.transform.rotation;
        targetRotation = startRotation * Quaternion.Euler(0, 180, 0);

        while (elapsedTime < 2f)
        {
            CameraRig.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / 2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        CameraRig.transform.rotation = targetRotation;

        elapsedTime = 0f;
        startRotation = CameraRig.transform.rotation;
        targetRotation = startRotation * Quaternion.Euler(0, 90, 0);

        while (elapsedTime < 1f)
        {
            CameraRig.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        CameraRig.transform.rotation = targetRotation;
        TryButton.SetActive(true);
    }

    public void SetComfortModeToNone()
    {
        gvm.gameSettings.ComfortModeOption = GameSettingsIO.comfortMode.None;
        SetButtonsStatus(0);
    }

    public void SetComfortModeToVignette()
    {
        gvm.gameSettings.ComfortModeOption = GameSettingsIO.comfortMode.Vignette;
        SetButtonsStatus(1);
    }

    public void SetComfortModeToPassthrough()
    {
        gvm.gameSettings.ComfortModeOption = GameSettingsIO.comfortMode.Passthrough;
        SetButtonsStatus(2);
    }

    private void SetButtonsStatus(int ButtonIndex)
    {
        NoneButton.interactable = true;
        VignetteButton.interactable = true;
        PassthroughButton.interactable = true;
        NoneButtonText.text = "Select";
        VignetteButtonText.text = "Select";
        PassthroughButtonText.text = "Select";
        switch (ButtonIndex)
        {
            case 0:
                NoneButton.interactable = false;
                NoneButtonText.text = "Selected";
                VignetteStrengthPanel.SetActive(false);
                break;
            case 1:
                VignetteButton.interactable = false;
                VignetteButtonText.text = "Selected";
                VignetteStrengthPanel.SetActive(true);
                ChangeVignetteSlider(gvm.gameSettings.VignetteStrength);
                break;
            case 2:
                PassthroughButton.interactable = false;
                PassthroughButtonText.text = "Selected";
                VignetteStrengthPanel.SetActive(false);
                break;
        }
    }

    public void SetVignetteStrength(float strength)
    {
        gvm.gameSettings.VignetteStrength = strength;
    }

    private void ChangeVignetteSlider(float strength)
    {
        VignetteStrengthText.text = strengthText.Replace("%0", strength.ToString());
        VignetteStrengthSlider.SetValueWithoutNotify(strength);
    }

    private void OnComfortSettingChanged(GameSettingsIO gameSettings)
    {
        switch (gameSettings.ComfortModeOption)
        {
            case GameSettingsIO.comfortMode.None:
                SetButtonsStatus(0);
                break;
            case GameSettingsIO.comfortMode.Vignette:
                SetButtonsStatus(1);
                break;
            case GameSettingsIO.comfortMode.Passthrough:
                SetButtonsStatus(2);
                break;
        }
    }
}