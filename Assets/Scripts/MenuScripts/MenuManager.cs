using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using Oculus.Interaction;

public class MenuManager : MonoBehaviour
{
    public GameObject menu;
    public Renderer menuRenderer;
    public float distanceFromPlayer = 1f;
    public Slider vignetteSlider;
    public Toggle[] movementToggles;
    public Toggle[] comfortToggles;
    public RayInteractable[] rayInteractablesToRestart;
    public TextMeshProUGUI vignetteStrengthText;
    public TextMeshProUGUI cameraHeightText;
    private Transform _camera;
    private GlobalVariableManager gvm;
    private bool _isMenuOpen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (menu == null) Debug.LogError("Menu object is not set in the inspector!");
        CloseMenu();
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        _camera = Camera.main.transform;
        OnSettingChanged(gvm.gameSettings);
        OnComfortSettingChanged(gvm.gameSettings);
        gvm.gameSettings.OnSettingChanged += OnSettingChanged;
        gvm.gameSettings.OnComfortSettingChanged += OnComfortSettingChanged;
    }

    private void OnSettingChanged(GameSettingsIO settings)
    {
        SetCameraHeightText(gvm.gameSettings.CameraHeight * 100);
        if (movementToggles.Length > 0) movementToggles[(int)gvm.gameSettings.MoveModeOption].isOn = true;
    }

    private void OnComfortSettingChanged(GameSettingsIO settings)
    {
        if (vignetteSlider != null) vignetteSlider.SetValueWithoutNotify(gvm.gameSettings.VignetteStrength);
        SetVignetteStrengthText(gvm.gameSettings.VignetteStrength);
        if (comfortToggles.Length > 0) comfortToggles[(int)gvm.gameSettings.ComfortModeOption].isOn = true;
    }

    public void RefreshMenu()
    {
        OnSettingChanged(gvm.gameSettings);
        OnComfortSettingChanged(gvm.gameSettings);
    }

    private void CheckMenuIsVisible()
    {
        if (!menu.activeSelf) return;
        if (!menuRenderer.isVisible) MoveMenuToPlayerSmoothly();
        if (Math.Abs(_camera.position.y - transform.position.y) > 0.1f) MoveMenuToPlayerSmoothly();
        float distance = Vector3.Distance(transform.position, _camera.position);
        if (distance > 2 * distanceFromPlayer)
        {
            MoveMenuToPlayerSmoothly();
        }
        // else if (_isMenuMoving) transform.DOKill();
    }

    void Update()
    {
        // If the player presses the "Menu" button on the left controller, open or close the menu
        if (OVRInput.GetDown(OVRInput.Button.Start)) ToggleMenu();
        if (_isMenuOpen) CheckMenuIsVisible();
    }

    public void CloseMenu()
    {
        _isMenuOpen = false;
        menu.SetActive(false);
    }

    private void OpenMenu(Vector3 offset = default)
    {
        _isMenuOpen = true;
        menu.SetActive(true);
        RestartRayInteractables();
        MoveMenuToPlayer(offset);
    }

    // Restart the ray interactables can solve the problem that slider jump back to the original position when gliding outside of the menu
    // **Note: I don't know why this works, but it just works :D
    private void RestartRayInteractables()
    {
        foreach (RayInteractable rayInteractable in rayInteractablesToRestart)
        {
            rayInteractable.Disable();
            rayInteractable.Enable();
        }
    }

    public void ToggleMenu()
    {
        if (_isMenuOpen) CloseMenu();
        else OpenMenu();
    }

    public void OpenMenuInFixPosition(Vector3 position, Vector3 rotation)
    {
        if (_isMenuOpen) CloseMenu();
        menu.SetActive(true);
        RestartRayInteractables();
        transform.position = position;
        transform.rotation = Quaternion.Euler(rotation);
        _isMenuOpen = true;
    }

    public void MoveMenuToPlayer(Vector3 offset = default)
    {
        Vector3 forwardOnXZ = new Vector3(_camera.forward.x, 0, _camera.forward.z).normalized;
        Vector3 targetPosition = _camera.position + forwardOnXZ * distanceFromPlayer;
        targetPosition += offset;
        transform.position = targetPosition;
        transform.LookAt(_camera.position);
    }

    public void MoveMenuToPlayerSmoothly(Vector3 offset = default)
    {
        Vector3 forwardOnXZ = new Vector3(_camera.forward.x, 0, _camera.forward.z).normalized;
        Vector3 targetPosition = _camera.position + forwardOnXZ * distanceFromPlayer;
        targetPosition += offset;

        // If the coroutine is already running, stop it
        transform.DOKill();

        transform.DOMove(targetPosition, 3f)
            .SetSpeedBased()
            .SetEase(Ease.OutCubic)
            .OnUpdate(() =>
            {
                transform.LookAt(_camera.position);
            });
    }

    private void SetVignetteStrengthText(float strength)
    {
        vignetteStrengthText.text = "Vignette Strength: %0".Replace("%0", strength.ToString());
    }

    private void SetCameraHeightText(float height)
    {
        cameraHeightText.text = "Camera Height: %0".Replace("%0", MetersToFeetInches(height));
    }

    public static string MetersToFeetInches(float centimeters)
    {
        int totalInch = (int)Math.Floor(centimeters / 2.54);
        int feet = totalInch / 12;
        int remainingInches = totalInch % 12;

        return $"{feet}ft{remainingInches}in";
    }

    public void ResetCameraHeight()
    {
        FindFirstObjectByType<CameraHeightManager>().ResetCameraHeight();
        MoveMenuToPlayerSmoothly();
    }

    void OnDestroy()
    {
        gvm.gameSettings.OnSettingChanged -= OnSettingChanged;
        gvm.gameSettings.OnComfortSettingChanged -= OnComfortSettingChanged;
    }
}