using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioPlayer : MonoBehaviour
{
    public AudioSource ASource;
    public AudioClip AOutdoors;
    public AudioClip AInside;
    private Rigidbody rb;

    private string outside = "outside";
    private string inside = "inside";

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude > 0)
        {
            typeGround();
        }
    }

    void typeGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
        {
            if (hit.collider.CompareTag("Exterior"))
            {
                playFootStepsAudio(AOutdoors);
            }
        }
    }

    void playFootStepsAudio(AudioClip audio)
    {
        ASource.pitch = 0.5f;
        //ASource.PlayOneShot(audio);
    }
}
