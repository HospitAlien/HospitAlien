using UnityEngine;

public class OneTimeSyringeBehaviour : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        //We only want to interact with the alien if the status of the syringe is ready
        if (collision.gameObject.CompareTag("Alien"))
        {
            collision.gameObject.SendMessage("Syrined");
            Destroy(this.gameObject);
        }

    }
}
