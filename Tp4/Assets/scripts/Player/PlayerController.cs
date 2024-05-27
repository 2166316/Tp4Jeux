using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f; 

    private Rigidbody rb;
    private Animator animator;

    //private Vector3 spawnPoint = new Vector3(35f, 66f, -77f);
    private Vector3 spawnPoint = new Vector3(9.717109f, 68f, -163f);
    private NetworkVariable<Vector3> posNetwork = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Rpc(SendTo.Server)]
    public void KillPlayerRpc()
    {
        isDead.Value = true;
        Debug.Log("Player died");
    }

    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer || !IsOwner)
            return;

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        //callback
        posNetwork.OnValueChanged += OnCurrentSpawn;
        if (IsOwner)
        {
            SpawnClientRPC();
        }

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

    void FixedUpdate()
    {
        if(!IsOwner || !IsSpawned)
            return;

        //si mort
        if (isDead.Value)
            return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized * moveSpeed;
        rb.MovePosition(rb.position + transform.TransformDirection(movement) * Time.fixedDeltaTime);

        float speed = movement.magnitude / moveSpeed;
        animator.SetFloat("movespeed", speed);

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        //corriger player par terre 
       /* Vector3 currentRotation = transform.rotation.eulerAngles;
      
        currentRotation.x = 0f;
        currentRotation.z = 0f;
      
        transform.rotation = Quaternion.Euler(currentRotation);

        MovePlayerServerRpc(transform.position, transform.rotation);*/
        // Debug.Log(transform.rotation.z +"  "+ transform.rotation.x);
    }

    [ServerRpc]
    private void MovePlayerServerRpc(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
        {
            return true;
        }
        return false;
    }
}
