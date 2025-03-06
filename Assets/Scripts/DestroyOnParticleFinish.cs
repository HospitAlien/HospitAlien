using UnityEngine;

public class DestroyOnParticleFinish : MonoBehaviour
{
    private ParticleSystem particleSystem;

    void Start()
    {
        // Get the ParticleSystem component
        particleSystem = GetComponent<ParticleSystem>();
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
