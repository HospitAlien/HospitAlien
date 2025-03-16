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

            GameObject newShrapnel = Instantiate(shrapnel, newPosition, Quaternion.identity);
            newShrapnel.transform.SetParent(transform); // Set the shrapnel as a child of the player

            count++;
        }
    }
}