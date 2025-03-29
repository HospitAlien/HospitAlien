using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MusicDataAndManualControl : MonoBehaviour
{
    [Header("Game Manager Reference")]
    public GameManager gameManager;  // Reference to your GameManager

    [Header("UI Elements for Music Control")]
    public Slider trackSelectionSlider;   // Slider for selecting track (0, 1, or 2)
    
    // Global Tempo slider (applies to all tracks)
    public Slider tempoSlider;            // For tempo control (e.g., 60 to 200 BPM)
    
    // Track 0 (Song 1) UI elements
    public Slider trackOneMelodySlider;   // For Song 1 melody volume
    public Slider trackOneDrumsSlider;    // For Song 1 drums volume

    // Track 1 (Song 2) UI elements
    public Slider trackTwoMelodySlider;   // For Song 2 melody volume
    public Slider trackTwoDrumsSlider;    // For Song 2 drums volume
    public Slider trackTwoBassSlider;     // For Song 2 bass volume

    // Track 2 (Song 3) UI elements
    public Slider hiPassSlider;           // For Song 3 hi pass volume
    public Slider lowPassSlider;          // For Song 3 low pass volume

    public Button saveDataButton;         // Button to trigger saving data

    [Header("Audio Sources for Each Track")]
    // Song 1 Audio Sources
    public AudioSource track1Melody;
    public AudioSource track1Drums;

    // Song 2 Audio Sources
    public AudioSource track2Melody;
    public AudioSource track2Drums;
    public AudioSource track2Bass;

    // Song 3 Audio Sources
    public AudioSource track3HiPass;
    public AudioSource track3LowPass;

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
            // Updated CSV header with descriptive column names.
            string header = "PatientCount,InjuryCount,TimeLeft,ActiveTrack,Tempo,Song1_MelodyVol,Song1_DrumsVol,Song2_MelodyVol,Song2_DrumsVol,Song2_BassVol,Song3_HiPassVol,Song3_LowPassVol";
            File.WriteAllText(csvFilePath, header + "\n");
        }
        
        // Configure the track selection slider for whole numbers (0 to 2).
        trackSelectionSlider.wholeNumbers = true;
        trackSelectionSlider.minValue = 0;
        trackSelectionSlider.maxValue = 2;
        trackSelectionSlider.value = 0;  // Start with Song 1 selected.

        // Set initial values for the global tempo slider.
        tempoSlider.value = 120f; // default BPM

        // Set initial values for Song 1 (Track 0)
        trackOneMelodySlider.value = 0.5f;
        trackOneDrumsSlider.value = 0.5f;

        // Set initial values for Song 2 (Track 1)
        trackTwoMelodySlider.value = 0.5f;
        trackTwoDrumsSlider.value = 0.5f;
        trackTwoBassSlider.value = 0.5f;

        // Set initial values for Song 3 (Track 2)
        hiPassSlider.value = 0.5f;
        lowPassSlider.value = 0.5f;

        // Set up listeners for UI events.
        trackSelectionSlider.onValueChanged.AddListener(delegate { UpdateTrack(); });
        saveDataButton.onClick.AddListener(SaveData);

        // Initialize the active track and slider visibility.
        UpdateTrack();
    }

    void Update()
    {
        // Update the active track's audio properties in real time.
        UpdateAudioProperties();
    }

    // Stops all audio sources.
    private void StopAllTracks()
    {
        if (track1Melody != null) track1Melody.Stop();
        if (track1Drums != null) track1Drums.Stop();
        if (track2Melody != null) track2Melody.Stop();
        if (track2Drums != null) track2Drums.Stop();
        if (track2Bass != null) track2Bass.Stop();
        if (track3HiPass != null) track3HiPass.Stop();
        if (track3LowPass != null) track3LowPass.Stop();
    }

    // Update which track is playing based on the track selection slider.
    private void UpdateTrack()
    {
        int selected = Mathf.RoundToInt(trackSelectionSlider.value);
        StopAllTracks();

        if (selected == 0)
        {
            if (track1Melody != null) track1Melody.Play();
            if (track1Drums != null) track1Drums.Play();
        }
        else if (selected == 1)
        {
            if (track2Melody != null) track2Melody.Play();
            if (track2Drums != null) track2Drums.Play();
            if (track2Bass != null) track2Bass.Play();
        }
        else if (selected == 2)
        {
            if (track3HiPass != null) track3HiPass.Play();
            if (track3LowPass != null) track3LowPass.Play();
        }

        // Update which sliders are visible based on the selected track.
        UpdateSliderVisibility();
    }

    // Update the audio properties (pitch and volume) of the active track.
    private void UpdateAudioProperties()
    {
        int selected = Mathf.RoundToInt(trackSelectionSlider.value);
        // Calculate pitch from the global tempo slider.
        float pitch = tempoSlider.value / baseBPM;

        if (selected == 0) // Song 1: tempo, melody, and drums.
        {
            if (track1Melody != null)
            {
                track1Melody.pitch = pitch;
                track1Melody.volume = trackOneMelodySlider.value;
            }
            if (track1Drums != null)
            {
                track1Drums.pitch = pitch;
                track1Drums.volume = trackOneDrumsSlider.value;
            }
        }
        else if (selected == 1) // Song 2: tempo, melody, drums, and bass.
        {
            if (track2Melody != null)
            {
                track2Melody.pitch = pitch;
                track2Melody.volume = trackTwoMelodySlider.value;
            }
            if (track2Drums != null)
            {
                track2Drums.pitch = pitch;
                track2Drums.volume = trackTwoDrumsSlider.value;
            }
            if (track2Bass != null)
            {
                track2Bass.pitch = pitch;
                track2Bass.volume = trackTwoBassSlider.value;
            }
        }
        else if (selected == 2) // Song 3: tempo, hi pass, and low pass.
        {
            if (track3HiPass != null)
            {
                track3HiPass.pitch = pitch;
                track3HiPass.volume = hiPassSlider.value;
            }
            if (track3LowPass != null)
            {
                track3LowPass.pitch = pitch;
                track3LowPass.volume = lowPassSlider.value;
            }
        }
    }

    // Updates slider visibility based on the selected track.
    private void UpdateSliderVisibility()
    {
        int selected = Mathf.RoundToInt(trackSelectionSlider.value);

        // For Song 1 (Track 0): Show only Song 1 sliders.
        if (selected == 0)
        {
            trackOneMelodySlider.gameObject.SetActive(true);
            trackOneDrumsSlider.gameObject.SetActive(true);

            trackTwoMelodySlider.gameObject.SetActive(false);
            trackTwoDrumsSlider.gameObject.SetActive(false);
            trackTwoBassSlider.gameObject.SetActive(false);
            hiPassSlider.gameObject.SetActive(false);
            lowPassSlider.gameObject.SetActive(false);
        }
        // For Song 2 (Track 1): Show only Song 2 sliders.
        else if (selected == 1)
        {
            trackOneMelodySlider.gameObject.SetActive(false);
            trackOneDrumsSlider.gameObject.SetActive(false);

            trackTwoMelodySlider.gameObject.SetActive(true);
            trackTwoDrumsSlider.gameObject.SetActive(true);
            trackTwoBassSlider.gameObject.SetActive(true);
            hiPassSlider.gameObject.SetActive(false);
            lowPassSlider.gameObject.SetActive(false);
        }
        // For Song 3 (Track 2): Show only Song 3 sliders.
        else if (selected == 2)
        {
            trackOneMelodySlider.gameObject.SetActive(false);
            trackOneDrumsSlider.gameObject.SetActive(false);

            trackTwoMelodySlider.gameObject.SetActive(false);
            trackTwoDrumsSlider.gameObject.SetActive(false);
            trackTwoBassSlider.gameObject.SetActive(false);
            hiPassSlider.gameObject.SetActive(true);
            lowPassSlider.gameObject.SetActive(true);
        }
    }

    // Save the current game state and music parameters to the CSV file.
    public void SaveData()
    {
        // Retrieve game parameters.
        int numPatients = gameManager.GetNumberOfPatients();
        int totalInjuries = gameManager.GetTotalInjuries();
        float totalTimeLeft = gameManager.GetTotalTimeLeft();

        int selected = Mathf.RoundToInt(trackSelectionSlider.value);
        float tempo = tempoSlider.value;

        // Set up volume variables for each song's controls.
        float track1MelodyVol = 0f;
        float track1DrumsVol = 0f;
        float track2MelodyVol = 0f;
        float track2DrumsVol = 0f;
        float track2BassVol = 0f;
        float track3HiPassVol = 0f;
        float track3LowPassVol = 0f;

        // Depending on the selected track, capture the relevant parameters.
        if (selected == 0)
        {
            track1MelodyVol = trackOneMelodySlider.value;
            track1DrumsVol = trackOneDrumsSlider.value;
        }
        else if (selected == 1)
        {
            track2MelodyVol = trackTwoMelodySlider.value;
            track2DrumsVol = trackTwoDrumsSlider.value;
            track2BassVol = trackTwoBassSlider.value;
        }
        else if (selected == 2)
        {
            track3HiPassVol = hiPassSlider.value;
            track3LowPassVol = lowPassSlider.value;
        }

        // Build the CSV row using the new descriptive column order.
        string row = $"{numPatients},{totalInjuries},{totalTimeLeft},{selected},{tempo},{track1MelodyVol},{track1DrumsVol},{track2MelodyVol},{track2DrumsVol},{track2BassVol},{track3HiPassVol},{track3LowPassVol}";
        File.AppendAllText(csvFilePath, row + "\n");

        Debug.Log("Data saved to " + csvFilePath);
    }
}
