using Oculus.Interaction.HandGrab;
using UnityEngine;

public class FireExtinguisherInputForwarder : MonoBehaviour, IHandGrabUseDelegate
{
    [SerializeField] private FireExtinguisher _fireExtinguisher;

    public void BeginUse()
    {
        if (_fireExtinguisher != null)
        {
            _fireExtinguisher.BeginUse();
        }
    }

    public void EndUse()
    {
        if (_fireExtinguisher != null)
        {
            _fireExtinguisher.EndUse();
        }
    }

    public float ComputeUseStrength(float strength)
    {
        if (_fireExtinguisher != null)
        {
            return _fireExtinguisher.ComputeUseStrength(strength);
        }
        return 0f;
    }
}
