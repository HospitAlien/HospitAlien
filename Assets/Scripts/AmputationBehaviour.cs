using UnityEngine;

public class AmputationBehaviour : MonoBehaviour
{
    private Alien alienBehaviour;
    private int leftHandCount = 0;
    private int rightHandCount = 0;
    private int leftLegCount = 0;
    private int rightLegCount = 0;

    GameObject bloodPrefab;
    private ParticleSystem[] bloodParticlesArray = new ParticleSystem[4];
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

    public void CreateCollider()
    {
        for (int i = 0; i < 4; i++)
        {
            if (alienBehaviour.status.needsAmputation[i])
            {
                GameObject colliderObject = null;
                Vector3 offset = Vector3.zero;
                Vector3 size = Vector3.zero;

                switch (i)
                {
                    case 0:
                        colliderObject = new GameObject("LeftHand");
                        offset = new Vector3(-0.62f, 0.81f, -0.04f);
                        size = new Vector3(0.22f, 0.08f, 0.23f);
                        break;
                    case 1:
                        colliderObject = new GameObject("RightHand");
                        offset = new Vector3(0.62f, 0.81f, -0.04f);
                        size = new Vector3(0.22f, 0.08f, 0.23f);
                        break;
                    case 2:
                        colliderObject = new GameObject("LeftLeg");
                        offset = new Vector3(-0.27f, 0.37f, -0.01f);
                        size = new Vector3(0.25f, 0.08f, 0.26f);
                        break;
                    case 3:
                        colliderObject = new GameObject("RightLeg");
                        offset = new Vector3(0.27f, 0.37f, -0.01f);
                        size = new Vector3(0.25f, 0.08f, 0.26f);
                        break;
                }

                if (colliderObject != null)
                {
                    // Make the new GameObject a child of the current GameObject.
                    colliderObject.transform.parent = transform;
                    // Set the local position to the offset originally used for the collider center.
                    colliderObject.transform.localPosition = offset;

                    // Add the BoxCollider component. Since the object is positioned correctly,
                    // we can set its center to zero.
                    BoxCollider boxCollider = colliderObject.AddComponent<BoxCollider>();
                    boxCollider.center = Vector3.zero;
                    boxCollider.size = size;

                    // Instantiate the blood particle effect for this limb
                    GameObject bloodObject = Instantiate(bloodPrefab, colliderObject.transform);
                    // Place the blood particle object at the collider's position (local position zero)
                    bloodObject.transform.localPosition = Vector3.zero;
                    // Save the ParticleSystem into the bloodParticlesArray
                    bloodParticlesArray[i] = bloodObject.GetComponent<ParticleSystem>();
                }
            }
        }
    }
}
