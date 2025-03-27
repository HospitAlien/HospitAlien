using UnityEngine;
using Oculus.Interaction.HandGrab;
using System.Collections;

public class Raygun : MonoBehaviour, IHandGrabUseDelegate
{
    public float fireRate;
    public LineRenderer rayPrefab;
    public Transform shootingPoint;
    public float range = 10f;

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
        LineRenderer ray = Instantiate(rayPrefab);
        ray.positionCount = 2; //start and an enpoint
        ray.SetPosition(0,shootingPoint.position);

        Vector3 endPoint = shootingPoint.position + shootingPoint.forward * range;
        ray.SetPosition(1,endPoint);

        Destroy(ray.gameObject, 0.5f);

        yield return new WaitForSeconds(1 / fireRate);
        onCooldown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (shooting && !onCooldown)
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
