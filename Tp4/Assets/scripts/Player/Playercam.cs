using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Playercam : NetworkBehaviour
{
    private float sensitivity = 100f;
    public GameObject player;

    private float rotationY = 0f;
    private float rotationX = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Camera controls
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        rotationY += mouseY;
        rotationX += mouseX;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);
        transform.localRotation = Quaternion.Euler(-rotationY, 0f, 0f);
        player.transform.localRotation = Quaternion.Euler(0f, rotationX, 0f);
    }
}
