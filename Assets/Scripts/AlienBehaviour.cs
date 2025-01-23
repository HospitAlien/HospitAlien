using UnityEngine;

public class AlienBehaviour : MonoBehaviour
{
    public Transform target;
    private Rigidbody rb;
    private Vector3 targetLocation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        targetLocation = new Vector3(target.position.x, transform.position.y,target.position.z);

    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 direction = (targetLocation - transform.position).normalized;
            float distanceToTarget =  Vector3.Distance( transform.position,targetLocation);

            float stoppingDistance = 0.1f;
            if (distanceToTarget > stoppingDistance)
            {
                float speed = 3f;
                rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
            }
            else
            {
                rb.MovePosition(targetLocation);
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
