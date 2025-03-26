using UnityEngine;

public class Cheating : MonoBehaviour
{
    public GameManager gameManager;

    void Update()
    {
        if (Application.isEditor)
        {
            if (OVRInput.GetDown(OVRInput.Button.Two)) gameManager.score += 1000;
            if (OVRInput.GetDown(OVRInput.Button.One)) gameManager.score -= 1000;
            gameManager.UpdateScoreText();
        }
    }
}
