using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 20f;
    private float moveSpeedY = 5f;
    private float moveX;
    private float moveY;

    private Transform orientation;
    private Vector3 moveDirection;
    private Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * moveY + orientation.right * moveX;
    }
}
