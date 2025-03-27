using UnityEngine;

public class AttachHandGreen : MonoBehaviour
{
    private Alien alienBehaviour;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        alienBehaviour = GetComponentInParent<Alien>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {

        Collider hitCollider = collision.contacts[0].thisCollider;
        string colliderName = hitCollider.gameObject.name;

        switch (colliderName)
        {
            case "LeftHandNeeded":
                Debug.Log("Left hand triggered");

                if (collision.gameObject.CompareTag("GreenHand"))
                {
                    Destroy(transform.Find("LeftHandNeeded").gameObject);
                    alienBehaviour.attachLimb(collision.gameObject, 0);
                }
                break;
            case "RightHandNeeded":
                Debug.Log("Right hand triggered");

                if (collision.gameObject.CompareTag("GreenHand"))
                {
                    Destroy(transform.Find("RightHandNeeded").gameObject);
                    alienBehaviour.attachLimb(collision.gameObject, 1);
                }
                break;
            case "LeftLegNeeded":
                Debug.Log("Left leg triggered");

                if (collision.gameObject.CompareTag("GreenLeg"))
                {
                    Destroy(transform.Find("LeftLegNeeded").gameObject);
                    alienBehaviour.attachLimb(collision.gameObject, 2);
                }
                break;
            case "RightLegNeeded":
                Debug.Log("Right leg triggered");
                if (collision.gameObject.CompareTag("GreenLeg"))
                {
                    Destroy(transform.Find("RightLegNeeded").gameObject);
                    alienBehaviour.attachLimb(collision.gameObject, 3);
                }
                break;
            default:
                break;


        }

    }
}
