using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 3.5f;
    public float jumpForce = 4f;
    private float multiplier = 1f;
    private Rigidbody rb;
    private Animator animator;

    //private Vector3 spawnPoint = new Vector3(35f, 66f, -77f);
    private Vector3 spawnPoint = new Vector3(9.717109f, 66f, -163f);
    private NetworkVariable<Vector3> posNetwork = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private Camera playerCam;
    private AudioListener playerAudio;
    private Camera debutCam;
    private AudioListener debutAudio;
    private GameObject loginPanel;
    private bool isGrounded;

    private AudioPlayer audioPlayer;


    [Rpc(SendTo.Server)]
    public void KillPlayerRpc()
    {
        isDead.Value = true;
        animator.SetBool("isDead", true);
        gameObject.tag = "Untagged";
        Debug.Log("Player died");
        audioPlayer.stopStepsAudio();
    }

    public bool getIsDeadVal()
    {
        return isDead.Value;
    }

    //vue login
    public void GoToConnection()
    {
        if (playerCam != null)
        {
            //reset all juste pour �tre s�r
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
            //reset all juste pour �tre s�r
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

    void Start()
    {
        audioPlayer = GetComponent<AudioPlayer>();
        // audioPlayer.playWalkingStepsAudio();
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
        isGrounded = true;
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

        if (NetworkManager.Singleton.IsHost )
        {
            NetworkManager.Singleton.Shutdown();
        }
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
        if (!IsOwner || !IsSpawned)
            return;

        //si mort
        if (isDead.Value)
            return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");


        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized * (moveSpeed * multiplier);
        rb.MovePosition(rb.position + transform.TransformDirection(movement) * Time.fixedDeltaTime);
        float speed = (movement.magnitude);
        animator.SetFloat("Blend", speed);

        //Debug.Log(animator.GetFloat("Blend"));
        if (speed >= 6f)
        {
            if (!(audioPlayer.currentClipPlaying == ClipPlaying.running))
            {
                audioPlayer.playRunningStepsAudio();
            }
        }
        else if (speed >= 1)
        {
            if (!(audioPlayer.currentClipPlaying == ClipPlaying.walking))
            {
                audioPlayer.playWalkingStepsAudio();
            }
        }
        else
        {
            if (!(audioPlayer.currentClipPlaying == ClipPlaying.idle))
            {
                audioPlayer.stopStepsAudio();
            }
        }
    }


    private void Update()
    {
        if (!IsOwner || !IsSpawned)
            return;

        //si mort
        if (isDead.Value)
            return;

        //jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        //run
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // Debug.Log("test");
            multiplier = 1.5f;
        }
        else
        {
            multiplier = 1f;
        }
    }

    public void Jump()
    {
        // Debug.Log("jump");

        //stop animation marche
        animator.speed = 0f;
        animator.SetFloat("Blend", 0);
        //animation saut
        animator.speed = 1f;
        animator.SetBool("isJump", true);

        StartCoroutine(TimeBeforeNextJump());
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    private IEnumerator TimeBeforeNextJump()
    {
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("isJump", false);

        yield return new WaitForSeconds(0.7f);
        isGrounded = true;
    }
}
