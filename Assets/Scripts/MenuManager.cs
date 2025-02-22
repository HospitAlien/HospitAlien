using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    public GameObject menu;
    public Renderer menuRenderer;
    public float distanceFromPlayer = 0.8f;
    public Slider vignetteSlider;
    private Transform _camera;
    private GlobalVariableManager gvm;
    private bool _isMenuOpen;
    private int _menuNotVisibleCounter = 99;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (menu == null) Debug.LogError("Menu object is not set in the inspector!");
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        if (vignetteSlider != null) vignetteSlider.SetValueWithoutNotify(gvm.gameSettings.VignetteStrength); // Init the slider value
        _camera = Camera.main.transform;
        OpenMenu();
    }

    private void CheckMenuIsVisible()
    {
        if (!menu.activeSelf) return;
        if (!menuRenderer.isVisible) _menuNotVisibleCounter++;
        else _menuNotVisibleCounter = 0;
        if (_menuNotVisibleCounter > 4) MoveMenuSmoothly();
    }

    void Update()
    {
        // If the player presses the "Menu" button on the left controller, open or close the menu
        if (OVRInput.GetDown(OVRInput.Button.Start)) ToggleMenu();
    }

    void LateUpdate()
    {
        float distance = Vector3.Distance(transform.position, _camera.position);
        if (distance > 2.5)
        {
            MoveMenuSmoothly();
        }
    }

    private void CloseMenu()
    {
        _isMenuOpen = false;
        menu.SetActive(false);
        CancelInvoke(nameof(CheckMenuIsVisible));
    }

    private void OpenMenu()
    {
        _isMenuOpen = true;
        MoveMenu();
        menu.SetActive(true);
        InvokeRepeating(nameof(CheckMenuIsVisible), 0f, 0.5f);
    }

    public void ToggleMenu(bool forceOpen = false)
    {
        if (forceOpen && _isMenuOpen) CloseMenu();
        if (_isMenuOpen) CloseMenu();
        else OpenMenu();
    }

    public void MoveMenu()
    {
        Vector3 targetPosition = _camera.position + _camera.forward * distanceFromPlayer;

        transform.position = targetPosition;
        transform.LookAt(_camera.position);
    }

    public void MoveMenuSmoothly()
    {
        Vector3 targetPosition = _camera.position + _camera.forward * distanceFromPlayer;

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

    // Used by buttons in the menu
    public void UpdateVignetteStrength(float strength)
    {
        gvm.gameSettings.VignetteStrength = strength;
    }

    public void UpdateGameStatus(bool start)
    {
        gvm.IsGamePlaying = start;
    }

    public class a
    {
        public int b;

        public a(int b)
        {
            this.b = b;
        }
    }
}