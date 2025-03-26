using UnityEngine;
using Oculus.Interaction.HandGrab;
using System.Collections;

public class Raygun : MonoBehaviour, IHandGrabUseDelegate
{
    public float fireRate;

    private bool onCooldown = false;
    private bool shooting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    IEnumerator Shoot()
    {
        onCooldown = true;
        Debug.Log("SHOOTING");
        yield return new WaitForSeconds(1/fireRate);
        onCooldown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(shooting && !onCooldown)
        {
            StartCoroutine(Shoot());
        }
    }

    public void BeginUse()
    {
        Debug.Log("GOT HERE");
        shooting = true;
    }

    public float ComputeUseStrength(float strength)
    {
        return strength;
    }

    public void EndUse()
    {
        shooting = false;
    }
}
