using UnityEngine;
using Unity.Barracuda;

public class MusicManager : MonoBehaviour
{
    public GameManager gameManager;
    
    // Reference to the model controller that runs the NN 
    public MusicModelController modelController;
    
    public AudioSource calmTrack;
    public AudioSource intenseTrack;
    
    // Base BPM used for adjusting pitch (assumes 120 BPM is the base)
    public float baseBPM = 120f;
    void Start() {
        //  start playing calm track by default.
        calmTrack.Play();
        intenseTrack.Stop();
    }

    void Update()
    {
        float numberOfPatients = gameManager.GetNumberOfPatients();
        float totalInjuries = gameManager.GetTotalInjuries();
        float totalTimeLeft = gameManager.GetTotalTimeLeft();
        Debug.Log($"num patients: {numberOfPatients}, total injuries {totalInjuries}, totaltimeleft: {totalTimeLeft}");
        
        // Evaluate the neural network model with these parameters.
        // The model should output three values:
        //   - outputs[0]: A decision value for which track to play
        //   - outputs[1]: The tempo (in BPM, e.g., between 60 and 200)
        //   - outputs[2]: The overall volume (0 to 1)
        float[] outputs = modelController.EvaluateModel(numberOfPatients, totalInjuries, totalTimeLeft);
        Debug.Log($"Model Outputs -> Decision: {outputs[0]}, Tempo: {outputs[1]}, Volume: {outputs[2]}");

        float musicTrackDecision = outputs[0];
        float tempo = outputs[1];
        float volume = outputs[2];
        
        if (musicTrackDecision > 0.5f)
        {
            if (!intenseTrack.isPlaying)
            {
                Debug.Log("intense starting, calm stopping");
                intenseTrack.Play();
                calmTrack.Stop();
            }
            intenseTrack.pitch = tempo / baseBPM;
            intenseTrack.volume = volume;
            Debug.Log($"Intense Track - Pitch: {intenseTrack.pitch}, Volume: {intenseTrack.volume}");

        }
        else
        {
            if (!calmTrack.isPlaying)
            {
                Debug.Log("calm starting, intense stopping");
                calmTrack.Play();
                intenseTrack.Stop();
            }
            calmTrack.pitch = tempo / baseBPM;
            calmTrack.volume = volume;
            Debug.Log($"Calm Track - Pitch: {calmTrack.pitch}, Volume: {calmTrack.volume}");

        }
    }
}
