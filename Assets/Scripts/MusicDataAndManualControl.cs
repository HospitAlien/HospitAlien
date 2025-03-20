using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MusicDataAndManualControl : MonoBehaviour
{
    [Header("Game Manager Reference")]
    public GameManager gameManager;  // Reference to your GameManager

    [Header("UI Elements for Music Control")]
    public Slider trackSelectionSlider;   // Slider for selecting track (0 or 1)
    public Slider volumeSlider;             // Slider for volume (range 0 to 1)
    public Slider tempoSlider;              // Slider for tempo (e.g., 60 to 200 BPM)
    public Button saveDataButton;           // Button to trigger saving data

    [Header("Audio Sources for Each Track")]
    public AudioSource track0;       // AudioSource for the first track
    public AudioSource track1;       // AudioSource for the second track

    // CSV file path
    private string csvFilePath;

    // Base BPM for calculating pitch adjustments
    private float baseBPM = 120f;

    void Start()
    {
        // Define the CSV file path in the persistent data directory.
        csvFilePath = Path.Combine(Application.persistentDataPath, "music_dataset.csv");
        if (!File.Exists(csvFilePath))
        {
            string header = "numPatients,totalInjuries,totalTimeLeft,track,volume,tempo";
            File.WriteAllText(csvFilePath, header + "\n");
        }
        
        // Configure the track selection slider to use whole numbers (only 0 and 1).
        trackSelectionSlider.wholeNumbers = true;
        trackSelectionSlider.minValue = 0;
        trackSelectionSlider.maxValue = 1;
        trackSelectionSlider.value = 0;  // Start with track0 selected.

        // Set initial values for the volume and tempo sliders.
        volumeSlider.value = 0.5f;
        tempoSlider.value = 120f;

        // Set up listeners for UI events.
        trackSelectionSlider.onValueChanged.AddListener(delegate { UpdateTrack(); });
        saveDataButton.onClick.AddListener(SaveData);

        // Initialize the active track.
        UpdateTrack();
    }

    void Update()
    {
        // Update the active track's volume and pitch in real time.
        UpdateAudioProperties();
    }

    // Update which track is playing based on the slider value.
    private void UpdateTrack()
    {
        // Stop both tracks before starting the selected one.
        track0.Stop();
        track1.Stop();

        // Get the selected track from the slider (0 or 1).
        int selected = Mathf.RoundToInt(trackSelectionSlider.value);
        if (selected == 0)
        {
            track0.Play();
        }
        else
        {
            track1.Play();
        }
    }

    // Adjust volume and pitch (tempo) of the currently active track.
    private void UpdateAudioProperties()
    {
        // Calculate pitch based on the tempo slider (tempo divided by baseBPM).
        float pitch = tempoSlider.value / baseBPM;
        
        // Determine the active track based on the slider value.
        int selected = Mathf.RoundToInt(trackSelectionSlider.value);
        AudioSource activeTrack = (selected == 0) ? track0 : track1;
        if (activeTrack != null)
        {
            activeTrack.pitch = pitch;
            activeTrack.volume = volumeSlider.value;
        }
    }

    // Save the current game and music parameters to the CSV file.
    public void SaveData()
    {
        // Retrieve game parameters from the GameManager.
        int numPatients = gameManager.GetNumberOfPatients();
        int totalInjuries = gameManager.GetTotalInjuries();
        float totalTimeLeft = gameManager.GetTotalTimeLeft();

        // Retrieve current music settings from the UI.
        int track = Mathf.RoundToInt(trackSelectionSlider.value);
        float volume = volumeSlider.value;
        float tempo = tempoSlider.value;

        // Build a CSV row.
        string row = $"{numPatients},{totalInjuries},{totalTimeLeft},{track},{volume},{tempo}";
        File.AppendAllText(csvFilePath, row + "\n");

        Debug.Log("Data saved to " + csvFilePath);
    }
}
