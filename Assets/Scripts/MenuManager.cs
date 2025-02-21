using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject menu;
    public float distanceFromPlayer = 0.5f;
    public Vector3 additionalOffset = Vector3.zero;
    public Slider vignetteSlider;
    private Transform _camera;
    private GlobalVariableManager _gvm;
    private bool _isMenuOpen;
    private IEnumerator _moveMenuCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (menu == null) Debug.LogError("Menu object is not set in the inspector!");
        _gvm = FindFirstObjectByType<GlobalVariableManager>();
        if (_gvm == null) Debug.LogError("GlobalVariableManager is not found in the scene!");
        if (vignetteSlider != null) vignetteSlider.SetValueWithoutNotify(_gvm.gameSettings.VignetteStrength); // Init the slider value
        _camera = Camera.main.transform;
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(CheckMenuIsVisible), 0f, 1f);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(CheckMenuIsVisible));
    }

    private void CheckMenuIsVisible()
    {
        if (!menu.activeSelf) return;

        if (!menu.GetComponent<Renderer>().isVisible) MoveMenuSmoothly();
    }

    // Update is called once per frame
    void Update()
    {
        // If the player presses the "Menu" button on the left controller, open or close the menu
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            if (_isMenuOpen) CloseMenu();
            else OpenMenu();
        }
        CheckMenuIsVisible();
    }

    public void OpenMenu()
    {
        MoveMenu();
        menu.SetActive(true);
        _isMenuOpen = true;
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        _isMenuOpen = false;
    }

    public void MoveMenu()
    {
        Vector3 targetPosition = _camera.position + _camera.forward * distanceFromPlayer;
        // Add additional offset to the target position
        targetPosition += _camera.TransformDirection(additionalOffset);

        menu.transform.position = targetPosition;
        menu.transform.LookAt(_camera.position);
    }

    public void MoveMenuSmoothly()
    {
        Vector3 targetPosition = _camera.position + _camera.forward * distanceFromPlayer;
        // Add additional offset to the target position
        targetPosition += _camera.TransformDirection(additionalOffset);
        if (_moveMenuCoroutine != null) StopCoroutine(_moveMenuCoroutine);
        _moveMenuCoroutine = MoveMenuCoroutine(targetPosition);
        StartCoroutine(_moveMenuCoroutine);
    }

    private IEnumerator MoveMenuCoroutine(Vector3 targetPosition)
    {
        Vector3 startPosition = menu.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.5f;
            menu.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            menu.transform.LookAt(_camera.position);
            yield return null;
        }

        menu.transform.position = targetPosition;
    }

    public void UpdateVignetteStrength(float strength)
    {
        _gvm.gameSettings.VignetteStrength = strength;
    }

    public void UpdateGameStatus(bool start)
    {
        _gvm.IsGamePlaying = start;
    }
}