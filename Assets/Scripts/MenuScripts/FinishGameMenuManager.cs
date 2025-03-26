using UnityEngine;
using TMPro;
using Oculus.Interaction;

public class FinishGameMenuManager : MonoBehaviour
{
    public GameObject menu;
    public Renderer menuRenderer;
    public float distanceFromPlayer = 1.2f;
    public RayInteractable[] rayInteractablesToRestart;
    private Transform _camera;
    private bool _isMenuOpen;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI restartTimeText;
    private GameManager gameManager;

    public int[] targetScores = new int[] { 0, 2500, 5000, 7500, 10000, 12500, 17500, 999999 };
    public string[] targetRanks = new string[]
        {
            "Medical Student",
            "Resident",
            "Fellow",
            "Attending Physician",
            "Department Head",
            "Medical Director",
            "Consultant Doctor",
            "Cheater"
        };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Start()
    {
        if (menu == null) Debug.LogError("Menu object is not set in the inspector!");
        CloseMenu();
        _camera = Camera.main.transform;
    }

    private void CloseMenu()
    {
        _isMenuOpen = false;
        menu.SetActive(false);
    }

    public void OpenMenu(Vector3 offset = default)
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

    public void MoveMenuToPlayer(Vector3 offset = default)
    {
        Vector3 forwardOnXZ = new Vector3(_camera.forward.x, 0, _camera.forward.z).normalized;
        Vector3 targetPosition = _camera.position + forwardOnXZ * distanceFromPlayer;
        targetPosition += offset;
        transform.position = targetPosition;
        transform.LookAt(_camera.position);
    }

    public void SetScoreText(int score)
    {
        scoreText.text = "Your score is: " + score;
        titleText.text = "You got title: " + GetTitle(score);
    }

    private string GetTitle(int score)
    {
        for (int i = 0; i < targetScores.Length; i++)
        {
            if (score < targetScores[i])
            {
                return targetRanks[i - 1];
            }
        }
        return targetRanks[targetRanks.Length - 1];
    }

    public void SetRestartTime(float newTime)
    {
        int minutes = (int)(newTime / 60);
        int seconds = (int)(newTime % 60);
        restartTimeText.text = $"The game will restart in: {minutes:D2}:{seconds:D2}";
    }

    public void ResetRestartTime()
    {
        gameManager.StartRestartCountdown();
    }
}