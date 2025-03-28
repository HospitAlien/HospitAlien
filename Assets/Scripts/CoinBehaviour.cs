using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{

    private int rateOverTime;
    private ParticleSystem _particleSystem;
    private AudioSource audioSource;

    void Start()
    {
        // Get the ParticleSystem component
        _particleSystem = GetComponent<ParticleSystem>();
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
        if (!_particleSystem.isPlaying)
        {
            // Destroy the GameObject after the particles finish
            Destroy(gameObject);
        }
        var emission = _particleSystem.emission;
        emission.rateOverTime = rateOverTime;
    }

    public void SetRate(int rate)
    {
        rateOverTime = rate;
    }
}
