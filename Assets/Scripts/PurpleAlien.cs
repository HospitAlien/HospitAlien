using Oculus.Interaction;
using UnityEngine;

public class PurpleAlien : Alien
{
   

    //TODO: I think its better to roll a random number to decide how many troubles the alien has then select from a list with weighted probabilities
    protected override void InitiateStatus()
    { 
        status.initiateStatus(this, gameManager.currentGameStage);
    }


    public void applyEyedrop(){
        status.applyEyeDrop();
        if(status.isHealthy()){
            Cure();
        }
    }

    protected override Material LoadLimbMaterial()
    {
        amputateMaterial = Resources.Load<Material>("AmputateGreen");
        return amputateMaterial;
    }

    //Add a child to hold colliders to detect axe hits
    public override void initiateAmputation()
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
        amputationDetector.AddComponent<AmputationBehaviourPurple>();

        for (int i = 0; i < status.needsAmputation.Length; i++)
        {
            if (status.needsAmputation[i])
            {
                switch (i)
                {
                    case 0:
                        ChangeLimbMaterial(transform.Find("body/hands/left_hand"));
                        break;
                    case 1:
                        ChangeLimbMaterial(transform.Find("body/hands/right_hand"));
                        break;
                    case 2:
                        ChangeLimbMaterial(transform.Find("body/feet/foot_left"));
                        break;
                    case 3:
                        ChangeLimbMaterial(transform.Find("body/feet/foot_right"));
                        break;

                }
            }
        }
    }

    protected override void initiateAttachHand()
    {
        Transform amputationDetectorTransform = transform.Find("AmputationDetector");

        //Add attachment script.
        if (amputationDetectorTransform.gameObject.GetComponent<AttachHandPurple>() == null)
        {
            amputationDetectorTransform.gameObject.AddComponent<AttachHandPurple>();
        }
    }


    public void initiateShrapnel()
    {
        int count = 0;
        while (count < 4)
        {
            // Use Unity's Random class for generating random numbers
            float offsetX = UnityEngine.Random.Range(-0.3f, 0.3f);
            float offsetY = UnityEngine.Random.Range(0f, 0.5f);
            float offsetZ = 0.1f;  // Depth offset (can be used to spawn metal further into the body)

            // Spawn the new shrapnel at the calculated position
            Vector3 newPosition = bodyTransform.position + new Vector3(offsetX, offsetY, offsetZ);

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