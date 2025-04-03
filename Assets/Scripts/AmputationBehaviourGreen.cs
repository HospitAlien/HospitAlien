using UnityEngine;

public class AmputationBehaviourGreen : AmputationBehaviour
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
                Quaternion rotation = Quaternion.identity;

                switch (i)
                {
                    case 0:
                        colliderObject = new GameObject("LeftHand");
                        offset = new Vector3(-0.1962f, 1.473f, 0.0048f);
                        size = new Vector3(0.05f, 0.13f, 0.09f);
                        break;
                    case 1:
                        colliderObject = new GameObject("RightHand");
                        offset = new Vector3(0.164f, 1.473f, 0.0048f);
                        size = new Vector3(0.05f, 0.13f, 0.09f);
                        break;
                    case 2:
                        colliderObject = new GameObject("LeftLeg");
                        offset = new Vector3(-0.0846f, 0.9766f, -0.002f);
                        size = new Vector3(0.05f, 0.2f, -0.21f);
                        rotation = Quaternion.Euler(0, 0, 35);
                        break;
                    case 3:
                        colliderObject = new GameObject("RightLeg");
                        offset = new Vector3(0.0653f, 0.9689f, -0.002f);
                        size = new Vector3(0.05f, 0.2f, -0.21f);
                        rotation = Quaternion.Euler(0, 0, -35);
                        break;
                }

                if (colliderObject != null)
                {
                    // Make the new GameObject a child of the current GameObject.
                    colliderObject.transform.parent = transform;
                    // Set the local position to the offset originally used for the collider center.
                    colliderObject.transform.localPosition = offset;
                    colliderObject.transform.localRotation = rotation;


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
