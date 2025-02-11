using UnityEngine;

public class AlienVoice : MonoBehaviour
{

    public GameObject TTS;
    private TTSSpeaker TTSScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (TTS != null)
        {
            TTSScript = TTS.GetComponent<TTSSpeaker>();
        }
        else
        {
            Debug.LogError("TTSSpeaker is not assigned in the inspector!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SayIllness()
    {
        Debug.Log("SayIllness Called");
        TTSScript.Speak("SayIllness Called");
    }
}
