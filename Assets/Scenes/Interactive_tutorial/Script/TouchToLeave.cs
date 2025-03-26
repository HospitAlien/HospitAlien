using UnityEngine;

// this script should attached to game setting menu to let buttons work
public class TouchToLeave : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Controller"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("HospitAlienSpaceShip");
        }
        Destroy(gameObject);
    }
}