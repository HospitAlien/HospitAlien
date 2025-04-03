using UnityEngine;
using Meta.WitAi.TTS.Utilities;
using Oculus.Voice;
using Meta.WitAi.Json;
using System.Collections;
using TMPro;

public class AlienVoice : MonoBehaviour
{

    public TTSSpeaker TTSScript;
    public AppVoiceExperience VoiceExperience;
    private Alien alien;

    public TMP_Text transcriptText;


    public string[] voicePresets = new string[]
    {
        "WIT$BRITISH BUTLER",
        "WIT$CARL",
        "WIT$CARTOON BABY",
        "WIT$CARTOON VILLAIN",
        "WIT$CHARLIE",
        "WIT$COLIN",
        "WIT$HOLLYWOOD",
        "WIT$PROSPECTOR",
        "WIT$REBECCA",
        "WIT$ROSIE",
        "WIT$SURFER",
        "WIT$WHIMSICAL",
        "WIT$VAMPIRE",
        "WIT$WIZARD"
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        alien = GetComponent<Alien>();

        //Randomise voice
        int randomIndex = Random.Range(0, voicePresets.Length);
        string selectedPreset = voicePresets[randomIndex];
        TTSScript.VoiceID = selectedPreset;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        TTSScript = GetComponentInChildren<TTSSpeaker>();
        if (TTSScript == null)
        {
            Debug.LogError("TTSSpeaker component not found in children!");
        }

        GameObject VoiceExperienceObject = GameObject.Find("App Voice Experience");
        if (VoiceExperienceObject != null)
        {
            VoiceExperience = VoiceExperienceObject.GetComponent<AppVoiceExperience>();
        }
        else
        {
            Debug.LogError("VoiceExperienceObject not found");
        }
    }


    [ContextMenu("Activate Listening")]
    public void ActivateListening()
    {
        if (VoiceExperience != null)
        {
            VoiceExperience.VoiceEvents.OnResponse.AddListener(HandleWitResponse);
            VoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(HandlePartialTranscription);

            VoiceExperience.Activate();

            transcriptText.text = "<sprite=0> Listening!";
            transcriptText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Voice Experience not linked to gameObject");
        }
    }

    public void Deactivate()
    {
        VoiceExperience.Deactivate();
        VoiceExperience.VoiceEvents.OnResponse.RemoveListener(HandleWitResponse);
        VoiceExperience.VoiceEvents.OnPartialTranscription.RemoveListener(HandlePartialTranscription);
        transcriptText.gameObject.SetActive(false);
    }

    private void HandleWitResponse(WitResponseNode response)
    {
        if (IntentMatches(response, "find_issue"))
        {
            SayIllness();
        }
        else if (IntentMatches(response, "greeting"))
        {
            TTSScript.Speak("I am in agony, help please");
        }
        else if (IntentMatches(response, "blood_type"))
        {
            string bloodType = alien.getBloodType();
            SayLine("I need" + bloodType + " blood!");
        }
        else
        {
            Debug.LogWarning("Intent doesn't match");
        }

        Deactivate();


    }



    public void SayIllness()
    {
        TTSScript.Speak(alien.getVoiceLine());
    }

    private bool IntentMatches(WitResponseNode response, string Intent)
    {
        var ReceivedIntent = response?["intents"]?[0]?["name"]?.Value;
        if (ReceivedIntent != null)
        {
            return string.Equals(ReceivedIntent, Intent, System.StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            Debug.LogError("Response Node when accessed returns null");
        }

        return false;
    }

    public void SayLine(string Line)
    {
        TTSScript.Speak(Line);
    }

    private void HandlePartialTranscription(string transcription)
    {
        if (transcriptText != null)
        {
            // Ensure the text is visible and prepend a sprite (adjust the sprite tag as needed)
            transcriptText.text = "<sprite=0> " + transcription;
        }
    }



}
