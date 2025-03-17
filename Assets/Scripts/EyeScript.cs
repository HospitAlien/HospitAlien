using UnityEngine;

public class EyeScript : MonoBehaviour
{
    public Material bloodShotMaterial;
    private Material normalEyeMaterial;
    private PurpleAlien alien;
    private bool active;
    public AudioSource sizzleSFX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void activate(PurpleAlien theAlien){
        Renderer renderer = GetComponent<Renderer>();
        normalEyeMaterial = renderer.material;
        renderer.material = bloodShotMaterial;
        alien = theAlien;
        active = true;
    }


    void OnParticleCollision(GameObject particle)
    {
        Debug.Log("HIT");

        if(particle.CompareTag("Eye-Dropper") && active){
            Renderer renderer = GetComponent<Renderer>();
            renderer.material = normalEyeMaterial;
            alien.applyEyedrop();
            sizzleSFX.Play();
            active = false;
        }
    }

    void OnCollisionEnter(Collision collision){
        Debug.Log("Hit");
    }
}
