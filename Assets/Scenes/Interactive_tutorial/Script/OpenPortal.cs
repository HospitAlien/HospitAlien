using UnityEngine;

public class OpenPortal : MonoBehaviour
{
    public GameObject[] ObjectToShow;
    public TutorialManager tutorialManager;
    private new Renderer renderer;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = renderer.material;
            if (material != null)
            {
                renderer.material = new Material(material);
            }
            else
            {
                Debug.LogError("Material not found");
                renderer = null;
            }
        }
        else
        {
            Debug.LogError("Renderer or Material not found");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Controller"))
        {
            renderer.material.color = Color.green;
        }
        for (int i = 0; i < ObjectToShow.Length; i++)
        {
            ObjectToShow[i].SetActive(true);
        }
        tutorialManager.Finish();
    }
}
