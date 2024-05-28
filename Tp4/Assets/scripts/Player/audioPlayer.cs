using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioPlayer : MonoBehaviour
{
    public AudioSource ASource;
    public AudioClip AOutdoors;
    public AudioClip AInside;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool typeGround()
    {
        RaycastHit hit;
        bool isgrounded = false;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
        {
            isgrounded = true;
        }
        return isgrounded;
    }
}
