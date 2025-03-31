using Oculus.Voice;
using TMPro;
using UnityEngine;

public class VoiceTranscription : MonoBehaviour
{
    // Assign these references via the Inspector
    public AppVoiceExperience appVoiceExperience;
    public TMP_Text transcriptionText;

    void Start()
    {
        // Hide text by default
        transcriptionText.enabled = false;

        if (appVoiceExperience != null)
        {
            // Subscribe to transcription events
            appVoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(OnPartialTranscription);
            appVoiceExperience.VoiceEvents.OnStoppedListening.AddListener(OnStoppedListening);
        }
    }

    // Called continuously as transcription is occurring
    void OnPartialTranscription(string transcription)
    {
        // Prepend an inline sprite icon to the transcription text.
        // Replace "index=0" with the appropriate index or use <sprite name="iconName"> if using named sprites.
        transcriptionText.text = $"<sprite index=0> {transcription}";
        transcriptionText.enabled = true;
    }

    // Called when transcription stops (or listening ends)
    void OnStoppedListening()
    {
        transcriptionText.enabled = false;
    }
}
