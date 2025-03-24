using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float speed = 0.1f;

    void Start() 
    {
        
    }


    void Update()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 directionToCamera = (cameraPosition - transform.position).normalized;
        transform.position += directionToCamera * speed * Time.deltaTime;
    }
    
}
