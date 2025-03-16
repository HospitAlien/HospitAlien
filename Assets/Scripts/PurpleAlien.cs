using UnityEngine;
public class PurpleAlien : Alien
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

        if(random.NextDouble() < 0.4){
            reward += 75;
            status.numberOfBadEyes = 1;
            int eyeNumber = random.Next(1,4);
            Debug.Log(eyeNumber);
            EyeScript eye = transform.Find($"eye{eyeNumber}").gameObject.GetComponent<EyeScript>();
            eye.activate(this);
        }


    }


    public void applyEyedrop(){
        status.applyEyeDrop();
        if(status.isHealthy()){
            Cure();
        }
    }


    private void initiateShrapnel()
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

    void FixedUpdate()
    {

        if (target != null)
        {


            // Check if the agent has reached the destination
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                //Move the alien to bed and disable path-finding
                agent.Warp(beds[target.position].Item1);
                agent.enabled = false;
                transform.rotation = beds[target.position].Item2;
                target = null;
                isReady = true;
                //Reposition timer so it's not on the floor (it's rotated alongside the alien")
                Transform timerText = transform.Find("TimerText");
                if (timerText != null)
                {
                    timerText.localPosition = new Vector3(0f, 0.15f, 0.06f);
                    timerText.localRotation = Quaternion.Euler(-90f, 180f, 0f);
                }
            }
            else
            {
                agent.SetDestination(targetLocation);
                isReady = false;
            }
        }
    }
}