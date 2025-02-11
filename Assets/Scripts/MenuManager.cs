using UnityEngine;

public class MenuManager : MonoBehaviour
{

    private GlobalVariableManager gvm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}