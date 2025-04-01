using UnityEngine;

// this script should attached to game setting menu to let buttons work
public class TutorialBucket : MonoBehaviour
{
    public GameObject Shrapnel1;
    public GameObject Shrapnel2;
    public TutorialManager tutorialManager;
    private int throwCount = 0;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Shrapnel"))
        {
            throwCount++;
            if (throwCount == 1)
            {
                Shrapnel1.SetActive(true);
            }
            else if (throwCount == 2)
            {
                Shrapnel2.SetActive(true);
            }
            if (throwCount == 3)
            {
                tutorialManager.FinishGrabAndThrow();
            }
        }
        Destroy(collider.gameObject);
    }
}