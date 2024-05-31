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

    private Camera playerCam;
    private AudioListener playerAudio;
    private Camera debutCam;
    private AudioListener debutAudio;
    private GameObject loginPanel;

    [Rpc(SendTo.Server)]
    public void KillPlayerRpc()
    {
        isDead.Value = true;
        animator.SetBool("isDead", true);
        gameObject.tag = "Untagged";
        Debug.Log("Player died");
    }

    //vue login
    public void GoToConnection()
    {
        if (playerCam != null)
        {
            //reset all juste pour être sûr
            debutAudio.enabled = false;
            debutCam.enabled = false;
            playerCam.enabled = false;
            playerAudio.enabled = false;

            debutCam.enabled = true;
            debutAudio.enabled = true;
        }

        //enable le login panel
        if (loginPanel != null)
        {
            loginPanel.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("loginPanel est null");
        }
    }

    //vue game
    public void GoToGame()
    {
        if (playerCam != null)
        {
            //reset all juste pour être sûr
            debutAudio.enabled = false;
            debutCam.enabled = false;
            playerCam.enabled = false;
            playerAudio.enabled = false;

            playerCam.enabled = true;
            playerAudio.enabled = true;
        }

        //disable le login panel
        if (loginPanel != null)
        {
            loginPanel.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("loginPanel est null");
        }
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

        //init des gameobjs 
        loginPanel = GameObject.FindGameObjectWithTag("LoginPanel");
        playerCam = GetComponentInChildren<Camera>();
        playerAudio = GetComponentInChildren<AudioListener>();
        debutCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        debutAudio = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioListener>();

        //vue jeux
        GoToGame();
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        //vue connection
        GoToConnection();
        base.OnNetworkDespawn();
    }

    public void OnCurrentSpawn(Vector3 previous, Vector3 current)
    {
        //change la position quand le networkvariable change
        transform.position = current;
    }

    //change la position du player au spawn network
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

        float speed = (movement.magnitude / moveSpeed) / 2;
        animator.SetFloat("Blend", speed);

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
