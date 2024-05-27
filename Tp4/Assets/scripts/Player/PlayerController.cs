using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private float moveSpeed = 5f;
    private float jumpForce = 50f; 

    private Rigidbody rb;
    private Animator animator;

    private Vector3 spawnPoint = new Vector3(1.8f, 65f, -168f);
    private NetworkVariable<Vector3> posNetwork = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Rpc(SendTo.Server)]
    public void KillPlayerRpc()
    {
        isDead.Value = true;
        die();
        Debug.Log("Player died");
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        //callback
        posNetwork.OnValueChanged += OnCurrentSpawn;
        if (IsOwner)
        {
            SpawnClientRPC();
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer || !IsOwner)
            return;
        
        Camera playerCam = GetComponentInChildren<Camera>();
        AudioListener playerAudio = GetComponentInChildren<AudioListener>();
        Camera debutCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        AudioListener debutAudio = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioListener>();
        if (playerCam != null)
        {
            debutAudio.enabled = false;
            debutCam.enabled = false;
            playerCam.enabled = false;
            playerAudio.enabled = false;

            playerCam.enabled = true;
            playerAudio.enabled = true;
        }

        GameObject loginPanel = GameObject.FindGameObjectWithTag("LoginPanel");
        if (loginPanel != null)
        {
            loginPanel.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("loginPanel est null");
        }
        
        base.OnNetworkSpawn();
    }

    public void OnCurrentSpawn(Vector3 previous, Vector3 current)
    {
        //change la position quand le networkvariable change
        transform.position = current;
    }

    [Rpc(SendTo.Server)]
    void SpawnClientRPC()
    {
        posNetwork.Value = spawnPoint;
    }

    void Update()
    {
        if (!IsOwner || isDead.Value)
            return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized * moveSpeed;
        rb.MovePosition(rb.position + transform.TransformDirection(movement) * Time.deltaTime);

        float speed = movement.magnitude / moveSpeed;
        animator.SetFloat("movespeed", speed);

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            animator.SetBool("isJump", true);
            Jump();
        }
        if (!IsGrounded())
        {
            animator.SetBool("isJump", false);
        }
    }

    void die()
    {
        //animator.SetBool("isDead", true);

    }

    void Jump()
    {
        Debug.Log("HIT");
        //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    bool IsGrounded()
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
