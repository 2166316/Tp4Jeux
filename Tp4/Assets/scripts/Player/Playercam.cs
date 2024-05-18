using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playercam : MonoBehaviour
{
    private float sensX = 50f;
    private float sensY = 50f;

    //public Transform orientation;
    public GameObject player;

    private float xRotation;
    private float yRotation;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //camera controles
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
        yRotation += mouseX;
        xRotation += mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        player.GetComponent<Transform>().rotation = Quaternion.Euler(0, yRotation, 0);
        //orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        //transform.position = player.GetComponent<Transform>().position + new Vector3(0, 3.5f, 0);
    }
}
