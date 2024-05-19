using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playercam : MonoBehaviour
{
    private float sensitivity = 100f;
    public GameObject player;

    private float rotationY = 0f; // This controls up-down rotation (Y-axis)
    private float rotationX = 0f; // This controls left-right rotation (X-axis)

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

        // Apply rotation to the camera for up-down movement (around X-axis)
        transform.localRotation = Quaternion.Euler(-rotationY, 0f, 0f); // Invert rotationY

        // Apply rotation to the player for left-right movement (around Y-axis)
        player.transform.localRotation = Quaternion.Euler(0f, rotationX, 0f); // Apply rotation to player, not camera
    }
}
