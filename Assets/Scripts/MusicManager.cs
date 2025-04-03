using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public GameManager gameManager;

    // Reference to the model controller that runs the NN.
    public MusicModelController modelController;

    // AudioSources for the individual song stems:
    // Song 1: keys and drums
    public AudioSource song1_keys;
    public AudioSource song1_drums;
    // Song 2: keys, drums, and bass
    public AudioSource song2_keys;
    public AudioSource song2_drums;
    public AudioSource song2_bass;
    // Song 3: hi pass and low pass
    public AudioSource song3_hiPass;
    public AudioSource song3_lowPass;

    // AudioSource for the pizza track
    public AudioSource pizzaTrack;

    // Base BPM used for adjusting pitch.
    public float baseBPM = 120f;

    // --- New variables for crossfade management ---
    // -1 indicates that no track is active yet.
    private int currentActiveTrack = -1;
    private bool isCrossfading = false;
    // Duration of the crossfade (in seconds)
    private float fadeDuration = 1f;

    void Start()
    {
        // Start playing all stems so they can be modulated.
        song1_keys.Play();
        song1_drums.Play();
        song2_keys.Play();
        song2_drums.Play();
        song2_bass.Play();
        song3_hiPass.Play();
        song3_lowPass.Play();

        // Ensure pizza track is stopped at start.
        pizzaTrack.Stop();
    }

    void Update()
    {
        // First, handle the pizza event case.
        float eventState = gameManager.GetEvent();
        if (eventState == 1 && gameManager.currentPatientCount == 0)
        {
            if (!pizzaTrack.isPlaying)
            {
                pizzaTrack.Play();
            }
            // Mute all other stems immediately.
            SetAllStemsVolume(0f);
            return;
        }
        else
        {
            if (pizzaTrack.isPlaying)
                pizzaTrack.Stop();
        }

        // Evaluate the NN.
        // Expected NN outputs:
        // [activeTrack, tempo, song1_keysVol, song1_drumsVol,
        //  song2_keysVol, song2_drumsVol, song2_bassVol,
        //  song3_hiPassVol, song3_lowPassVol]
        float numPatients = gameManager.GetNumberOfPatients();
        float totalInjuries = gameManager.GetTotalInjuries();
        float totalTimeLeft = gameManager.GetTotalTimeLeft();
        float[] outputs = modelController.EvaluateModel(numPatients, totalInjuries, totalTimeLeft);

        // Extract outputs.
        float activeTrackValue = outputs[0];
        float tempo = outputs[1];
        // For Song 1:
        float song1_keysTarget = outputs[2];
        float song1_drumsTarget = outputs[3];
        // For Song 2:
        float song2_keysTarget = outputs[4];
        float song2_drumsTarget = outputs[5];
        float song2_bassTarget = outputs[6];
        // For Song 3:
        float song3_hiPassTarget = outputs[7];
        float song3_lowPassTarget = outputs[8];

        // Calculate pitch.
        float pitchValue = tempo / baseBPM;

        // Decide which track to play based on activeTrack.
        // We round the activeTrack output to an integer (0, 1, or 2).
        int newTrackDecision = Mathf.Clamp(Mathf.RoundToInt(activeTrackValue), 0, 2);

        // If we are currently crossfading, skip direct volume updates.
        if (isCrossfading)
            return;

        // If the track decision has changed, initiate a crossfade.
        if (newTrackDecision != currentActiveTrack)
        {
            // Prepare target volumes for the new track.
            float[] newTargetVolumes = null;
            if (newTrackDecision == 0)
                newTargetVolumes = new float[] { song1_keysTarget, song1_drumsTarget };
            else if (newTrackDecision == 1)
                newTargetVolumes = new float[] { song2_keysTarget, song2_drumsTarget, song2_bassTarget };
            else if (newTrackDecision == 2)
                newTargetVolumes = new float[] { song3_hiPassTarget, song3_lowPassTarget };

            StartCoroutine(CrossfadeTracks(currentActiveTrack, newTrackDecision, newTargetVolumes, pitchValue));
            currentActiveTrack = newTrackDecision;
        }
        else
        {
            // No track change: update the active track's pitch and volumes instantly.
            AudioSource[] activeSources = GetAudioSourcesForTrack(currentActiveTrack);
            float[] targetVolumes = null;
            if (currentActiveTrack == 0)
                targetVolumes = new float[] { song1_keysTarget, song1_drumsTarget };
            else if (currentActiveTrack == 1)
                targetVolumes = new float[] { song2_keysTarget, song2_drumsTarget, song2_bassTarget };
            else if (currentActiveTrack == 2)
                targetVolumes = new float[] { song3_hiPassTarget, song3_lowPassTarget };

            // First, mute all stems.
            SetAllStemsVolume(0f);

            // Then update active stems.
            for (int i = 0; i < activeSources.Length; i++)
            {
                activeSources[i].pitch = pitchValue;
                activeSources[i].volume = targetVolumes[i];
                if (!activeSources[i].isPlaying)
                    activeSources[i].Play();
            }
        }
    }

    // Helper method to immediately set volume of all stems to a given value.
    void SetAllStemsVolume(float volume)
    {
        song1_keys.volume = volume;
        song1_drums.volume = volume;
        song2_keys.volume = volume;
        song2_drums.volume = volume;
        song2_bass.volume = volume;
        song3_hiPass.volume = volume;
        song3_lowPass.volume = volume;
    }

    // Returns the AudioSources belonging to a given track.
    AudioSource[] GetAudioSourcesForTrack(int track)
    {
        if (track == 0)
            return new AudioSource[] { song1_keys, song1_drums };
        else if (track == 1)
            return new AudioSource[] { song2_keys, song2_drums, song2_bass };
        else if (track == 2)
            return new AudioSource[] { song3_hiPass, song3_lowPass };
        return new AudioSource[0];
    }

    // Coroutine to crossfade from the old track to the new one over fadeDuration seconds.
    IEnumerator CrossfadeTracks(int oldTrack, int newTrack, float[] newTargetVolumes, float pitchValue)
    {
        isCrossfading = true;
        float timeElapsed = 0f;

        // Get old and new track sources.
        AudioSource[] oldSources = GetAudioSourcesForTrack(oldTrack);
        AudioSource[] newSources = GetAudioSourcesForTrack(newTrack);

        // For old sources, store their initial volumes.
        float[] oldInitialVolumes = new float[oldSources.Length];
        for (int i = 0; i < oldSources.Length; i++)
            oldInitialVolumes[i] = oldSources[i].volume;

        // Ensure new sources start at 0.
        foreach (AudioSource src in newSources)
        {
            src.volume = 0f;
            src.pitch = pitchValue;
            if (!src.isPlaying)
                src.Play();
        }

        while (timeElapsed < fadeDuration)
        {
            float t = timeElapsed / fadeDuration;
            // Fade out old track.
            for (int i = 0; i < oldSources.Length; i++)
            {
                oldSources[i].volume = Mathf.Lerp(oldInitialVolumes[i], 0f, t);
            }
            // Fade in new track.
            for (int i = 0; i < newSources.Length; i++)
            {
                float targetVol = newTargetVolumes[i];
                newSources[i].volume = Mathf.Lerp(0f, targetVol, t);
                newSources[i].pitch = pitchValue; // update pitch continuously.
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final volumes.
        for (int i = 0; i < oldSources.Length; i++)
            oldSources[i].volume = 0f;
        for (int i = 0; i < newSources.Length; i++)
        {
            newSources[i].volume = newTargetVolumes[i];
            newSources[i].pitch = pitchValue;
        }
        isCrossfading = false;
    }
}