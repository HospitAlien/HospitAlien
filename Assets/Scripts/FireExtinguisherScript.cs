using Oculus.Interaction.HandGrab;
using UnityEngine;

public class FireExtinguisher : MonoBehaviour, IHandGrabUseDelegate
{
    [Header("Extinguisher Settings")]
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField, Range(0f, 1f)] private float _releaseThreshold = 0.3f;
    [SerializeField, Range(0f, 1f)] private float _fireThreshold = 0.9f;
    [SerializeField] private float _triggerSpeed = 3f;
    [SerializeField] private AnimationCurve _strengthCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private bool _isFiring = false;
    private float _dampedUseStrength = 0f;
    private float _lastUseTime = 0f;

    private void Awake()
    {
        if (_particleSystem == null)
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }
    }

    // Called when the extinguisher starts being used.
    public void BeginUse()
    {
        _dampedUseStrength = 0f;
        _lastUseTime = Time.realtimeSinceStartup;
    }

    // Called continuously with the trigger input strength (0 to 1).
    public float ComputeUseStrength(float strength)
    {
        float delta = Time.realtimeSinceStartup - _lastUseTime;
        _lastUseTime = Time.realtimeSinceStartup;

        // Smooth the trigger input.
        if (strength > _dampedUseStrength)
        {
            _dampedUseStrength = Mathf.Lerp(_dampedUseStrength, strength, _triggerSpeed * delta);
        }
        else
        {
            _dampedUseStrength = strength;
        }

        float progress = _strengthCurve.Evaluate(_dampedUseStrength);
        UpdateFiringState(progress);
        return progress;
    }

    // Called when the extinguisher use ends.
    public void EndUse()
    {
        StopFiring();
    }

    // Checks the progress and starts or stops the ParticleSystem accordingly.
    private void UpdateFiringState(float progress)
    {
        if (progress >= _fireThreshold && !_isFiring)
        {
            _isFiring = true;
            StartFiring();
        }
        else if (progress <= _releaseThreshold && _isFiring)
        {
            _isFiring = false;
            StopFiring();
        }
    }

    // Starts the ParticleSystem.
    private void StartFiring()
    {
        if (_particleSystem != null && !_particleSystem.isPlaying)
        {
            _particleSystem.Play();
        }
    }

    // Stops the ParticleSystem.
    private void StopFiring()
    {
        if (_particleSystem != null && _particleSystem.isPlaying)
        {
            _particleSystem.Stop();
        }
    }
}
