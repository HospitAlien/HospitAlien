using UnityEngine;
using Oculus.Interaction.HandGrab;
using System.Collections;
using Oculus.Interaction;

public class Raygun : MonoBehaviour, IHandGrabUseDelegate
{
    public float fireRate;
    public LineRenderer rayPrefab;
    public Transform shootingPoint;
    public float range = 10f;
    public AudioSource shootSFX;
    public AudioClip sound;
    public DistanceGrabInteractable DistanceGrabInteractable;

    private bool pickedUp;

    private bool onCooldown = false;
    private bool shooting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        Vector3 newPosition = findPosition();


        transform.position = newPosition;
        pickedUp = false;
        DistanceGrabInteractable.WhenSelectingInteractorAdded.Action += GunHeld;
    }

    private void GunHeld(DistanceGrabInteractor interactor)
    {
        GetComponent<Rigidbody>().isKinematic = false;
        pickedUp = true;
    }

    IEnumerator Shoot()
    {
        onCooldown = true;
        LineRenderer ray = Instantiate(rayPrefab);
        ray.positionCount = 2; //start and an enpoint
        ray.SetPosition(0, shootingPoint.position);

        // Perform the raycast to find where the line hits
        RaycastHit hit;
        Vector3 endPoint;

        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out hit, range))
        {
            // If it hits something, set the endpoint to the hit point
            endPoint = hit.point;

            //if its a ghost we want to kill it
            Ghost ghost = hit.transform.GetComponentInParent<Ghost>();
            if (ghost)
            {
                ghost.GhostKilled();
            }
        }
        else
        {
            // If no hit, just use the maximum range
            endPoint = shootingPoint.position + shootingPoint.forward * range;
        }

        ray.SetPosition(1, endPoint);
        shootSFX.PlayOneShot(sound);

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

        else if (!pickedUp) //if its not picked up move it relative to the camera
        {

            transform.Rotate(Vector3.up, 30f * Time.deltaTime);
            Vector3 newPosition = findPosition();
            float moveSpeed = 5f;
            transform.position = Vector3.Lerp(transform.position, newPosition, moveSpeed * Time.deltaTime);
        }
    }

    private Vector3 findPosition()
    {
        float distanceFromCamera = 2.0f;
        Camera camera = Camera.main;
        Vector3 cameraPosition = camera.transform.position;
        Quaternion cameraRotation = camera.transform.rotation;

        // Calculate the new position in front of the camera
        Vector3 offset = cameraRotation * Vector3.forward * distanceFromCamera;
        Vector3 newPosition = cameraPosition + offset;
        return newPosition;
    }

    public void BeginUse()
    {
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
