using UnityEngine;
public class GreenAlien : Alien
{

    protected override void InitiateStatus()
    { 
        status.initiateStatus(this, gameManager.currentGameStage);
    }

    public void initiateShrapnel()
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