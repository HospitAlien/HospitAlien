using UnityEngine;

public class AmputationBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateCollider();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateCollider()
    {
        // Create the first BoxCollider with a specific center
        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.center = new Vector3(-0.38f, 1.23f, -0.01f);
        collider.size = new Vector3(0.13f, 0.2f, 0.23f);
    }

}
