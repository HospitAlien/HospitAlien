using UnityEngine;

public class AmputationBehaviour : MonoBehaviour
{
    protected Alien alienBehaviour;
    private int leftHandCount = 0;
    private int rightHandCount = 0;
    private int leftLegCount = 0;
    private int rightLegCount = 0;

    protected GameObject bloodPrefab;
    protected ParticleSystem[] bloodParticlesArray = new ParticleSystem[4];
    AudioSource audioSource;
    AudioClip hitSound;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        alienBehaviour = GetComponentInParent<Alien>();

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        bloodPrefab = Resources.Load<GameObject>("BloodSpurtParticles");
        CreateCollider();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 0.2f;
       
        hitSound = Resources.Load<AudioClip>("AxeHitSound");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
        

    // Counts up hits.
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Axe"))
        {
            Collider hitCollider = collision.contacts[0].thisCollider;
            string colliderName = hitCollider.gameObject.name;

            switch (colliderName)
            {
                case "LeftHand":
                    leftHandCount++;
                    Hit(leftHandCount, 0);
                    break;
                case "RightHand":
                    rightHandCount++;
                    Hit(rightHandCount, 1);
                    break;
                case "LeftLeg":
                    leftLegCount++;
                    Hit(leftLegCount, 2);
                    break;
                case "RightLeg":
                    rightLegCount++;
                    Hit(rightLegCount, 3);
                    break;


            }
        }
    }

    private void Hit(int hitCount, int limb)
    {
        audioSource.PlayOneShot(hitSound);
        bloodParticlesArray[limb].Play();
        Debug.Log("Hit by axe: " + hitCount);

        if (hitCount >= 5)
        {
            switch (limb)
            {
                case 0:
                    transform.Find("LeftHand").gameObject.name = "LeftHandNeeded";
                    break;
                case 1:
                    transform.Find("RightHand").gameObject.name = "RightHandNeeded";
                    break;
                case 2:
                    transform.Find("LeftLeg").gameObject.name = "LeftLegNeeded";
                    break;
                case 3:
                    transform.Find("RightLeg").gameObject.name = "RightLegNeeded";
                    break;
            }
            alienBehaviour.Amputate(limb);

        }
    }

    public virtual void CreateCollider()
    {
        Debug.Log("Create collider needs to be overridden");
    }
}
