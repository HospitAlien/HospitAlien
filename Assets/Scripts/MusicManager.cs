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
    
    void Update()
    {
        float numberOfPatients = gameManager.GetNumberOfPatients();
        float totalInjuries = gameManager.GetTotalInjuries();
        float totalTimeLeft = gameManager.GetTotalTimeLeft();
        
        // Evaluate the neural network model with these parameters.
        // The model should output three values:
        //   - outputs[0]: A decision value for which track to play
        //   - outputs[1]: The tempo (in BPM, e.g., between 60 and 200)
        //   - outputs[2]: The overall volume (0 to 1)
        float[] outputs = modelController.EvaluateModel(numberOfPatients, totalInjuries, totalTimeLeft);
        
        float musicTrackDecision = outputs[0];
        float tempo = outputs[1];
        float volume = outputs[2];
        
        if (musicTrackDecision > 0.5f)
        {
            if (!intenseTrack.isPlaying)
            {
                intenseTrack.Play();
                calmTrack.Stop();
            }
            intenseTrack.pitch = tempo / baseBPM;
            intenseTrack.volume = volume;
        }
        else
        {
            if (!calmTrack.isPlaying)
            {
                calmTrack.Play();
                intenseTrack.Stop();
            }
            calmTrack.pitch = tempo / baseBPM;
            calmTrack.volume = volume;
        }
    }
}
