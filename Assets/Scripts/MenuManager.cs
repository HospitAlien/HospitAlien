using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject menu;
    public Transform CenterEyeAnchor;
    private GlobalVariableManager gvm;
    private bool isMenuOpen;
    public float distanceFromPlayer = 0.5f;
    public float checkInterval = 2f; // Check every 2 seconds menu distance from player
    public float distanceThreshold = 2f;
    public Vector3 additionalOffset = Vector3.zero;
    public Slider vignetteSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isMenuOpen = false;
        if (menu != null)
        {
            menu.SetActive(false);
        }
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        if (vignetteSlider != null)
        {
            vignetteSlider.value = gvm.gameSettings.VignetteStrength;
        }
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(CheckMenuDistance), 0f, checkInterval);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(CheckMenuDistance));
    }

    private void CheckMenuDistance()
    {
        if (!menu.activeSelf) return;

        Vector3 horizontalOffset = new Vector3(
            menu.transform.position.x - CenterEyeAnchor.position.x,
            0,
            menu.transform.position.z - CenterEyeAnchor.position.z
        );

        float currentDistance = horizontalOffset.magnitude;

        // If the distance between the player and the menu is greater than the threshold, close the menu
        if (currentDistance > distanceThreshold)
        {
            menu.SetActive(false);
            isMenuOpen = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If the player presses the "B" button on the right controller, open or close the menu
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            if (!isMenuOpen)
            {
                MoveMenu();
                menu.SetActive(true);
                isMenuOpen = true;
            }
            else
            {
                menu.SetActive(false);
                isMenuOpen = false;
            }
        }
    }

    public void MoveMenu()
    {
        Vector3 targetPosition = CenterEyeAnchor.position + CenterEyeAnchor.forward * distanceFromPlayer;
        targetPosition += CenterEyeAnchor.TransformDirection(additionalOffset);
        menu.transform.position = targetPosition;
        menu.transform.LookAt(CenterEyeAnchor.position);
    }

    public void UpdateVignetteStrength(float strength)
    {
        gvm.gameSettings.VignetteStrength = strength;
        Debug.Log("Vignette strength updated to " + strength);
    }

    public void UpdateGameStatus(bool start)
    {
        gvm.IsGamePlaying = start;
    }
}