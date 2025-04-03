using UnityEngine;

// this script should attached to game setting menu to let buttons work
public class MainButtonEvents : MonoBehaviour
{
    private GlobalVariableManager gvm;

    void Start()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        if (gvm == null) Debug.LogError("GlobalVariableManager not found");
    }

    public void SetGameStart(bool start)
    {
        gvm.IsGamePlaying = start;
    }

    public void GoToSpaceShipScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("HospitAlienSpaceShip");
    }
}