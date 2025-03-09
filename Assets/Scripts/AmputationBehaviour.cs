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
        collider.center = new Vector3(-0.62f, 0.81f, -0.04f);
        collider.size = new Vector3(0.22f, 0.08f, 0.23f);
    }

}
