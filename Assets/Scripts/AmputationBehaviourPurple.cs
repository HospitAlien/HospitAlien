using UnityEngine;

public class AmputationBehaviourPurple : AmputationBehaviour
{

    public override void CreateCollider()
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
