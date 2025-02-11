using UnityEngine;
using Meta.WitAi.TTS.Utilities;
using Oculus.Voice;
using Meta.WitAi.Json;
using Meta.WitAi.Data.Configuration.Tabs;


public class AlienVoice : MonoBehaviour
{

    public TTSSpeaker TTSScript;
    public AppVoiceExperience VoiceExperience;
    public string Intent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        if(IntentMatches(response))
        {
            Debug.Log("Intent matches");
            SayIllness();
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
        TTSScript.Speak("SayIllness Called");
    }

    private bool IntentMatches(WitResponseNode response)
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
}
