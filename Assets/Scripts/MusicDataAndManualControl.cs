using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MusicDataAndManualControl : MonoBehaviour
{
    [Header("Game Manager Reference")]
    public GameManager gameManager;  // Make sure this is assigned in the Inspector

    [Header("UI Elements for Manual Control")]
    public Dropdown trackDropdown;   // Dropdown with options: 0 = Calm, 1 = Medium, 2 = Intense
    public Slider tuneVolumeSlider;  // Slider for tune volume (range 0 to 1)
    public Slider drumsVolumeSlider; // Slider for drums volume (range 0 to 1)
    public Slider tempoSlider;       // Slider for tempo (e.g., 60 to 200 BPM)
    public Button saveDataButton;    // Button to trigger saving data

    [Header("Audio Sources for Each Track")]
    public AudioSource calmTune;
    public AudioSource calmDrums;
    public AudioSource mediumTune;
    public AudioSource mediumDrums;
    public AudioSource intenseTune;
    public AudioSource intenseDrums;

    // CSV file path
    private string csvFilePath;

    // Base BPM for calculating pitch adjustments
    private float baseBPM = 120f;

    void Start()
    {
        // Set up CSV file path in persistent data folder.
        csvFilePath = Path.Combine(Application.persistentDataPath, "music_dataset.csv");
        if (!File.Exists(csvFilePath))
        {
            string header = "numPatients,totalInjuries,totalTimeLeft,track,tuneVolume,drumsVolume,tempo";
            File.WriteAllText(csvFilePath, header + "\n");
        }
        
        // Set initial slider values if desired
        tuneVolumeSlider.value = 0.5f;
        drumsVolumeSlider.value = 0.5f;
        tempoSlider.value = 120f;

        // Make sure the dropdown’s onValueChanged event calls UpdateTrack
        trackDropdown.onValueChanged.AddListener(delegate { UpdateTrack(); });
        
        // Add listener for the Save Data button
        saveDataButton.onClick.AddListener(SaveData);

        // Start by updating the active track based on the dropdown
        UpdateTrack();
    }

    void Update()
    {
        // Update the currently active track's properties every frame so you hear real-time changes.
        UpdateAudioProperties();
    }

    // Switch tracks based on the dropdown selection.
    private void UpdateTrack()
    {
        // Stop all tracks first.
        calmTune.Stop();
        calmDrums.Stop();
        mediumTune.Stop();
        mediumDrums.Stop();
        intenseTune.Stop();
        intenseDrums.Stop();

        // Play the selected track.
        int selected = trackDropdown.value;
        switch (selected)
        {
            case 0: // Calm track
                calmTune.Play();
                calmDrums.Play();
                break;
            case 1: // Medium track
                mediumTune.Play();
                mediumDrums.Play();
                break;
            case 2: // Intense track
                intenseTune.Play();
                intenseDrums.Play();
                break;
        }
    }

    // Update pitch and volume based on slider values.
    private void UpdateAudioProperties()
    {
        // Calculate the pitch based on the tempo slider.
        float pitch = tempoSlider.value / baseBPM;
        
        // Get references to the active track's audio sources.
        int selected = trackDropdown.value;
        AudioSource activeTune = null;
        AudioSource activeDrums = null;
        switch (selected)
        {
            case 0:
                activeTune = calmTune;
                activeDrums = calmDrums;
                break;
            case 1:
                activeTune = mediumTune;
                activeDrums = mediumDrums;
                break;
            case 2:
                activeTune = intenseTune;
                activeDrums = intenseDrums;
                break;
        }
        
        if (activeTune != null && activeDrums != null)
        {
            activeTune.pitch = pitch;
            activeDrums.pitch = pitch;
            activeTune.volume = tuneVolumeSlider.value;
            activeDrums.volume = drumsVolumeSlider.value;
        }
    }

    // This method grabs the current game state and manual music parameters, and then saves them as a CSV row.
    public void SaveData()
    {
        // Gather game parameters.
        int numPatients = gameManager.GetNumberOfPatients();
        int totalInjuries = gameManager.GetTotalInjuries();
        float totalTimeLeft = gameManager.GetTotalTimeLeft();

        // Gather current music settings from the UI.
        int track = trackDropdown.value;
        float tuneVolume = tuneVolumeSlider.value;
        float drumsVolume = drumsVolumeSlider.value;
        float tempo = tempoSlider.value;

        // Create the CSV row.
        string row = $"{numPatients},{totalInjuries},{totalTimeLeft},{track},{tuneVolume},{drumsVolume},{tempo}";
        File.AppendAllText(csvFilePath, row + "\n");
        Debug.Log("Data saved to " + csvFilePath);
    }
}
