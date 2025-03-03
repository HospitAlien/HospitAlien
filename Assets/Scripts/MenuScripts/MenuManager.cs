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
    private bool _isMenuMoving;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (menu == null) Debug.LogError("Menu object is not set in the inspector!");
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        if (vignetteSlider != null) vignetteSlider.SetValueWithoutNotify(gvm.gameSettings.VignetteStrength); // Init the slider value
        _camera = Camera.main.transform;
        OpenMenu(Vector3.down * 10f);
    }

    private void CheckMenuIsVisible()
    {
        if (!menu.activeSelf) return;
        if (!menuRenderer.isVisible) MoveMenuToPlayerSmoothly();
        else if (_isMenuMoving) transform.DOKill();
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
            MoveMenuToPlayer();
        }
    }

    private void CloseMenu()
    {
        CancelInvoke(nameof(CheckMenuIsVisible));
        _isMenuOpen = false;
        menu.SetActive(false);
    }

    private void OpenMenu(Vector3 offset = default)
    {
        _isMenuOpen = true;
        menu.SetActive(true);
        MoveMenuToPlayer(offset);
        InvokeRepeating(nameof(CheckMenuIsVisible), 2f, 0.1f);
    }

    public void ToggleMenu(bool forceOpen = false)
    {
        if (forceOpen && _isMenuOpen) CloseMenu();
        if (_isMenuOpen) CloseMenu();
        else OpenMenu();
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
}