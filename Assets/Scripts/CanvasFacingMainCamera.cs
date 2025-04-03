using UnityEngine;

public class CanvasFacingMainCamera : MonoBehaviour
{
    void Update()
    {
        if (Camera.main != null)
        {
            // Calculate direction from canvas to the camera
            Vector3 direction = Camera.main.transform.position - transform.position;
            // Rotate the canvas so its forward vector points toward the camera
            transform.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            Debug.Log("Canvas could not find main camera");
        }
    }
}
