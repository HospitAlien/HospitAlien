using Oculus.Interaction;
using UnityEngine;
public class GreenAlien : Alien
{


    //TODO: I think its better to roll a random number to decide how many troubles the alien has then select from a list with weighted probabilities
    protected override void InitiateStatus()
    {
        System.Random random = new System.Random();

        if (random.NextDouble() < 0.4)
        {
            Debug.Log("SWEATING");
            status.needsInjection = true;
            sweatParticles.Play();
            reward += 100;
        }
        if (random.NextDouble() < 0.4)
        {
            status.needsExtinguishing = true;
            Debug.Log("ON FIRE");
            fireParticles.Play();
            reward += 50;
        }

        if (random.NextDouble() < 0.4)
        {
            status.shrapnelCount = 4;
            reward += 150;
            status.hasShrapnel = true;
            initiateShrapnel();
        }

        if (random.NextDouble() < 1)
        {
            status.needsAnAmputation = true;
            status.needsAmputation = new bool[] { false, false, false, false };
            status.needsAttatchment = new bool[] { false, false, false, false }; //initially none of them need attatching it is only once a limb is removed
            initiateAmputation();
        }

        if (random.NextDouble() < 0.4)
        {
            reward += 100;
            if (random.NextDouble() < 0.5)
            { //shrink
                status.curSize = -1;
                transform.localScale /= 2f;
            }
            else
            { //enlargement
                status.curSize = 1;
                transform.localScale *= 1.4f;
            }
        }
    }

    protected override Material LoadLimbMaterial()
    {
        amputateMaterial = Resources.Load<Material>("AmputatePurple");
        return amputateMaterial;
    }

    protected override void initiateAmputation()
    {

        bool limbSet = false;
        System.Random random = new System.Random();
        for (int i = 0; i < status.needsAttatchment.Length; i++)
        {
            if (random.NextDouble() < 0.3)
            {
                status.needsAmputation[i] = true;
                limbSet = true;
            }
        }

        if (!limbSet)
        {
            status.needsAmputation[random.Next(0, 4)] = true;
        }


        // Create a new GameObject to detect axe hits
        GameObject amputationDetector = new GameObject("AmputationDetector");
        amputationDetector.transform.SetParent(transform);
        amputationDetector.transform.localPosition = Vector3.zero;
        amputationDetector.transform.localRotation = Quaternion.identity;
        amputationDetector.AddComponent<AmputationBehaviourGreen>();

        for (int i = 0; i < status.needsAmputation.Length; i++)
        {
            if (status.needsAmputation[i])
            {
                switch (i)
                {
                    case 0:
                        ChangeLimbMaterial(transform.Find("hands/left_hand"));
                        break;
                    case 1:
                        ChangeLimbMaterial(transform.Find("hands/right_hand"));
                        break;
                    case 2:
                        ChangeLimbMaterial(transform.Find("feet/foot_left"));
                        break;
                    case 3:
                        ChangeLimbMaterial(transform.Find("feet/foot_right"));
                        break;

                }
            }
        }
    }

    protected override void initiateAttachHand()
    {
        Transform amputationDetectorTransform = transform.Find("AmputationDetector");

        //Add attachment script.
        if (amputationDetectorTransform.gameObject.GetComponent<AttachHandGreen>() == null)
        {
            amputationDetectorTransform.gameObject.AddComponent<AttachHandGreen>();
        }
    }

    private void initiateShrapnel()
    {
        int count = 0;
        while (count < 4)
        {
            float offsetX = UnityEngine.Random.Range(-0.12f, 0.12f);
            float offsetY = UnityEngine.Random.Range(-0.2f, 0.2f);
            float offsetZ = 0.1f;

            // Spawn the new shrapnel at the calculated position
            Vector3 newPosition = bodyTransform.position + new Vector3(offsetX, offsetY, offsetZ);


            //also want to randomise the rotation
            float randomXRotation = UnityEngine.Random.Range(-30f, 30f);
            float randomZRotation = UnityEngine.Random.Range(0f, 360f);

            // Construct a new rotation with random x and z rotation values, keeping the y rotation the same as bodyTransform
            Quaternion randomRotation = Quaternion.Euler(randomXRotation, bodyTransform.rotation.eulerAngles.y, randomZRotation);

            GameObject newShrapnel = Instantiate(shrapnel, newPosition, randomRotation);
            newShrapnel.transform.SetParent(transform); // Set the shrapnel as a child of the player

            count++;
        }
    }
}