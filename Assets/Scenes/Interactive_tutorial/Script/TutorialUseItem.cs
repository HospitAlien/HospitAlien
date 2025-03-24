using Oculus.Interaction.HandGrab;
using UnityEngine;

public class TutorialUseItem : MonoBehaviour, IHandGrabUseDelegate
{
    private ParticleSystem foam;
    private AudioSource sound;
    private bool playSound = false;
    private bool showParticles = false;
    public TutorialManager tutorialManager;

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
    }

    public float ComputeUseStrength(float strength)
    {
        return strength;
    }

    public void EndUse()
    {
        if (showParticles) foam.Stop();
        if (playSound) sound.Stop();
        tutorialManager.FinishDistanceGrabUseTutorial();
    }
}
