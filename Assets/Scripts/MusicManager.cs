using UnityEngine;
using Unity.Barracuda;

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
    
    void Start() {
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
        float numPatients = gameManager.GetNumberOfPatients();
        float totalInjuries = gameManager.GetTotalInjuries();
        float totalTimeLeft = gameManager.GetTotalTimeLeft();
        float eventState = gameManager.GetEvent();

        // Log the game parameters.
        Debug.Log($"Game Params - Patients: {numPatients}, Injuries: {totalInjuries}, TimeLeft: {totalTimeLeft}, EventState: {eventState}");
        
        // Check for pizza event: if eventState == 1 and no patients.
        if (eventState == 1 && gameManager.currentPatientCount == 0)
        {
            if (!pizzaTrack.isPlaying)
            {
                Debug.Log("Pizza event active: starting pizza track, muting all other stems.");
                pizzaTrack.Play();
            }
            // Mute all other stems.
            song1_keys.volume = 0f;
            song1_drums.volume = 0f;
            song2_keys.volume = 0f;
            song2_drums.volume = 0f;
            song2_bass.volume = 0f;
            song3_hiPass.volume = 0f;
            song3_lowPass.volume = 0f;
        }
        else
        {
            // Ensure pizza track is muted.
            if (pizzaTrack.isPlaying)
                pizzaTrack.Stop();
            
            // Evaluate the NN.
            // Expected NN outputs:
            // [activeTrack, tempo, song1_keysVol, song1_drumsVol,
            //  song2_keysVol, song2_drumsVol, song2_bassVol,
            //  song3_hiPassVol, song3_lowPassVol]
            float[] outputs = modelController.EvaluateModel(numPatients, totalInjuries, totalTimeLeft);
            
            // Extract outputs.
            float activeTrackValue = outputs[0];
            float tempo = outputs[1];
            // For Song 1:
            float song1_keysVol = outputs[2];
            float song1_drumsVol = outputs[3];
            // For Song 2:
            float song2_keysVol = outputs[4];
            float song2_drumsVol = outputs[5];
            float song2_bassVol = outputs[6];
            // For Song 3:
            float song3_hiPassVol = outputs[7];
            float song3_lowPassVol = outputs[8];
            
            // Log NN outputs.
            Debug.Log($"NN Outputs - ActiveTrack: {activeTrackValue}, Tempo: {tempo}, " +
                      $"Song1 Keys: {song1_keysVol}, Song1 Drums: {song1_drumsVol}, " +
                      $"Song2 Keys: {song2_keysVol}, Song2 Drums: {song2_drumsVol}, Song2 Bass: {song2_bassVol}, " +
                      $"Song3 HiPass: {song3_hiPassVol}, Song3 LowPass: {song3_lowPassVol}");
            
            // Calculate pitch.
            float pitchValue = tempo / baseBPM;
            
            // Decide which track to play based on activeTrack.
            // We round the activeTrack output to an integer (0, 1, or 2).
            int trackDecision = Mathf.Clamp(Mathf.RoundToInt(activeTrackValue), 0, 2);
            Debug.Log("Active Track Decision: " + trackDecision);
            
            // Reset all volumes (they are all playing continuously).
            song1_keys.volume = 0f;
            song1_drums.volume = 0f;
            song2_keys.volume = 0f;
            song2_drums.volume = 0f;
            song2_bass.volume = 0f;
            song3_hiPass.volume = 0f;
            song3_lowPass.volume = 0f;
            
            // Apply pitch and volumes only to the chosen track.
            if (trackDecision == 0)
            {
                song1_keys.pitch = pitchValue;
                song1_keys.volume = song1_keysVol;
                song1_drums.pitch = pitchValue;
                song1_drums.volume = song1_drumsVol;
            }
            else if (trackDecision == 1)
            {
                song2_keys.pitch = pitchValue;
                song2_keys.volume = song2_keysVol;
                song2_drums.pitch = pitchValue;
                song2_drums.volume = song2_drumsVol;
                song2_bass.pitch = pitchValue;
                song2_bass.volume = song2_bassVol;
            }
            else if (trackDecision == 2)
            {
                song3_hiPass.pitch = pitchValue;
                song3_hiPass.volume = song3_hiPassVol;
                song3_lowPass.pitch = pitchValue;
                song3_lowPass.volume = song3_lowPassVol;
            }
            
            // Ensure that the stems of the active track are playing.
            // (They were started in Start() so we only need to check if they were muted/stopped.)
            if (trackDecision == 0)
            {
                if (!song1_keys.isPlaying) song1_keys.Play();
                if (!song1_drums.isPlaying) song1_drums.Play();
            }
            else if (trackDecision == 1)
            {
                if (!song2_keys.isPlaying) song2_keys.Play();
                if (!song2_drums.isPlaying) song2_drums.Play();
                if (!song2_bass.isPlaying) song2_bass.Play();
            }
            else if (trackDecision == 2)
            {
                if (!song3_hiPass.isPlaying) song3_hiPass.Play();
                if (!song3_lowPass.isPlaying) song3_lowPass.Play();
            }
        }
    }
}
