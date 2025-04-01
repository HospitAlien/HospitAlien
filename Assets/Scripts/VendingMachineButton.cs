using UnityEngine;

public class VendingButton : MonoBehaviour
{
    public GameObject objectPrefab;
    private new Renderer renderer;
    private bool isCoolingDown = false;
    private bool isPrinting = false;

    void Awake()
    {
        transform.parent.GetComponent<VendingMachine>().OnCoolDown += OnMachineCoolDown;
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
            if (isCoolingDown)
            {
                return;
            }
            isPrinting = true;
            transform.parent.GetComponent<VendingMachine>().SpawnObject(objectPrefab);
        }
    }

    private void OnMachineCoolDown(bool isCoolingDown)
    {
        this.isCoolingDown = isCoolingDown;
        if (renderer != null)
        {
            renderer.material.color = isCoolingDown ? Color.red : Color.white;
        }
        if (isPrinting && isCoolingDown) renderer.material.color = Color.green;
        isPrinting = false;
    }
}
