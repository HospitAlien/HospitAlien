using Oculus.Interaction.HandGrab;
using UnityEngine;

public class TutorialUseItem : MonoBehaviour, IHandGrabUseDelegate
{
    private ParticleSystem foam;
    private AudioSource sound;
    private bool playSound = false;
    private bool showParticles = false;
    public TutorialManager tutorialManager;
    private float usedTime = 0.0f;
    private float lastUseTime = 0.0f;

    private void Awake()
    {
        foam = GetComponent<ParticleSystem>();
        sound = GetComponent<AudioSource>();

        if (foam != null)
        {
            showParticles = true;
        }
        if (sound != null)
        {
            playSound = true;
        }
    }


    public void BeginUse()
    {
        if (showParticles) foam.Play();
        if (playSound) sound.Play();
        lastUseTime = Time.time;
    }

    public float ComputeUseStrength(float strength)
    {
        return strength;
    }

    public void EndUse()
    {
        if (showParticles) foam.Stop();
        if (playSound) sound.Stop();
        usedTime += Time.time - lastUseTime;
        if (usedTime > 2.0f) tutorialManager.FinishDistanceGrabUseTutorial();
    }
}
