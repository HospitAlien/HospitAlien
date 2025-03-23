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

    public override void attachLimb(GameObject bodyPart, int limb)
    {
        Transform amputationDetector = transform.Find("AmputationDetector");

        Destroy(bodyPart.transform.Find("ISDK_DistanceHandGrabInteraction").gameObject); //Prevent hand staying grabbable
        Destroy(bodyPart.GetComponent<RigidbodyKinematicLocker>());
        Destroy(bodyPart.GetComponent<Rigidbody>()); //Prevent gravity working on hand prior to attachment

        bodyPart.transform.SetParent(transform);
        bodyPart.transform.localScale = Vector3.one; //Adjust scale


        switch (limb)
        {
            case 0:
                bodyPart.transform.localPosition = new Vector3(1.686128f, -0.9940824f, 0.7895237f);
                bodyPart.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case 1:
                bodyPart.transform.localPosition = new Vector3(0.8970075f, -0.9940824f, 0.7895237f);
                bodyPart.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                break;

            case 2:
                bodyPart.transform.localPosition = new Vector3(0.0003096164f, 0.01475262f, 0f);
                bodyPart.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case 3:
                bodyPart.transform.localPosition = new Vector3(-0.0002702117f, 0.01395124f, 0f);
                bodyPart.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                break;

        }

        status.attachLimb(limb);

        status.needsAttatchment[limb] = false;

        if (status.isHealthy())
        {
            Cure();
        }
        else
        {
            alienVoice.SayLine("Thanks for the new hand!");
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