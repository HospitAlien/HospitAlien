using UnityEngine;
using UnityEngine.UI;  // For Slider
using Unity.Barracuda;

public class MusicInteraction : MonoBehaviour
{
    [Header("Sliders")]
    public Slider sliderSeverity;    // 0..100
    public Slider sliderPatients;    // 1..6
    public Slider sliderKnowledge;   // 0..10
    public Slider sliderTimeLeft;    // 0..298

    [Header("Music Model")]
    public MusicModelController modelController;

    [Header("Audio Sources")]
    public AudioSource baseAudio;         // baseline track
    public AudioSource percussionAudio;   // for percussion layer
    public AudioSource synthAudio;        // for synth layer
    public AudioSource stringsAudio;      // for strings layer

    // If you want separate highPercussion or just intensify existing percussion, add another source or script
    // public AudioSource highPercussionAudio;

    void Start()
    {
        // Optionally, you can set some default slider values here
        // sliderSeverity.value = 0f;
        // ...
    }

    void Update()
    {
        // 1) Get current slider values
        float severity = sliderSeverity.value;        // e.g., 0..100
        float patients = sliderPatients.value;        // e.g., 1..6
        float knowledge = sliderKnowledge.value;      // e.g., 0..10
        float timeLeft = sliderTimeLeft.value;        // e.g., 0..298

        // 2) Evaluate the model
        float[] outputs = modelController.EvaluateModel(severity, patients, knowledge, timeLeft);

        // 3) The 7 outputs
        float tempo             = outputs[0]; // 60..200
        float volume            = outputs[1]; // 0..1
        float percussionLevel   = outputs[2]; // 0..1
        float synthsLevel       = outputs[3]; // 0..1
        float intensity         = outputs[4]; // 0..1
        float useStrings        = outputs[5]; // 0..1 -> interpret as boolean
        float useHighPercussion = outputs[6]; // 0..1 -> interpret as boolean

        // 4) Apply to your audio system
        // a) Tempo -> adjust pitch
        //    We assume 120 is the "base" BPM in your audio clip
        baseAudio.pitch = tempo / 120f;

        // b) Overall volume
        baseAudio.volume = volume;

        // c) Percussion intensity -> adjust percussion volume
        percussionAudio.volume = percussionLevel;

        // d) Synth intensity -> adjust synth volume
        synthAudio.volume = synthsLevel;

        // e) Strings on/off
        if (useStrings > 0.5f)
            stringsAudio.volume = 1f;
        else
            stringsAudio.volume = 0f;

        // f) High percussion
        if (useHighPercussion > 0.5f)
        {
            // Either increase percussion volume or activate a second "high percussion" AudioSource
            // For example:
            percussionAudio.pitch = 1.2f; // a subtle pitch bump
        }
        else
        {
            percussionAudio.pitch = 1.0f;
        }
    }
}
