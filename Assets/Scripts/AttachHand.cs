using UnityEngine;

public class AttachHand : MonoBehaviour
{
    private AlienBehaviour alienBehaviour;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        alienBehaviour = GetComponentInParent<AlienBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("LeftHand"))
        {
            Debug.Log("Left hand detected");
            alienBehaviour.attachHand(collision.gameObject);
        }
    }
}
