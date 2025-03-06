using UnityEngine;

public class DestroyOnParticleFinish : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private AudioSource audioSource;

    void Start()
    {
        // Get the ParticleSystem component
        particleSystem = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();

        // Ensure the AudioSource is valid
        if (audioSource != null)
        {
            // Play the audio
            audioSource.time = 0.7f;
            audioSource.Play();
        }
    }

    void Update()
    {
        // Check if the particle system is still playing
        if (!particleSystem.isPlaying)
        {
            // Destroy the GameObject after the particles finish
            Destroy(gameObject);
        }
    }
}
