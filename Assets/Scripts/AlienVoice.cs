using UnityEngine;
using Meta.WitAi.TTS.Utilities;
using Oculus.Voice;
using Meta.WitAi.Json;
using System.Collections;

public class AlienVoice : MonoBehaviour
{

    public TTSSpeaker TTSScript;
    public AppVoiceExperience VoiceExperience;
    private Alien alien;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        alien = GetComponent<Alien>();

        // Warm up the voice connection
        if (VoiceExperience != null)
        {
            // Activate the connection early
            VoiceExperience.Activate();
            Debug.Log("WARMING UP CONNECTION");
            // Deactivate after a second delay
            StartCoroutine(WarmUpRoutine());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator WarmUpRoutine()
    {
        // Wait just enough time for the connection to be established
        yield return new WaitForSeconds(1f);
        VoiceExperience.Deactivate();
    }

    void Awake()
    {
        GameObject TTSSpeaker = GameObject.Find("TTSSpeaker");
        if (TTSSpeaker != null)
        {
            TTSScript = TTSSpeaker.GetComponent<TTSSpeaker>();
        }
        else
        {
            Debug.Log("TTSpeaker not found");
        }

        GameObject VoiceExperienceObject = GameObject.Find("App Voice Experience");
        if (VoiceExperienceObject != null)
        {
            VoiceExperience = VoiceExperienceObject.GetComponent<AppVoiceExperience>();
        }
        else
        {
            Debug.Log("VoiceExperienceObject not found");
        }
    }


    [ContextMenu("Activate Listening")]
    public void ActivateListening()
    {
        if (VoiceExperience != null)
        {
            Debug.Log("Activating VoiceExperience and listening");
            VoiceExperience.VoiceEvents.OnResponse.AddListener(HandleWitResponse);
            VoiceExperience.Activate();
        }
        else
        {
            Debug.Log("Voice Experience not linked to gameObject");
        }
    }

    private void HandleWitResponse(WitResponseNode response)
    {
        Debug.Log("WitResponse received by handler!");

        if (IntentMatches(response, "find_issue"))
        {
            Debug.Log("Intent matches");
            SayIllness();
        }
        else if (IntentMatches(response, "greeting"))
        {
            Debug.Log("Greeting recognised");
            TTSScript.Speak("I am in agony, help please");
        }
        else
        {
            Debug.Log("Intent doesn't match");
        }
        VoiceExperience.Deactivate();

        VoiceExperience.VoiceEvents.OnResponse.RemoveListener(HandleWitResponse);

    }

    public void SayIllness()
    {
        Debug.Log("SayIllness Called");
        TTSScript.Speak(alien.getVoiceLine());
    }

    private bool IntentMatches(WitResponseNode response, string Intent)
    {
        var ReceivedIntent = response?["intents"]?[0]?["name"]?.Value;
        Debug.Log(ReceivedIntent);
        Debug.Log(Intent);
        if (ReceivedIntent != null)
        {
            return string.Equals(ReceivedIntent, Intent, System.StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            Debug.Log("Response Node when accessed returns null");
        }

        return false;
    }

    public void SayLine(string Line){
        Debug.Log("Say line called");
        TTSScript.Speak(Line);
    }
}
