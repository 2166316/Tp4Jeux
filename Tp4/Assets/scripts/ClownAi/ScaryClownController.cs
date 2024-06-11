using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class ScaryClownController : NetworkBehaviour
{
    private NavMeshAgent navAgent;

    private Animator animator;
    private const string SPEED = "Speed";
    private int animatorVitesseHash;
    private const string MENACING = "Menacing";
    private int animatorMenacingHash;

    [SerializeField] private int clownSpeed = 10;

    private Vector3 destination;

    private AudioSource audioSource;

    private List<NetworkObject> players = new();

    private NetworkVariable<bool> lookingMenacing = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    private SpawnClownAI scaryClownSpawner;
    public override void OnNetworkSpawn()
    {
         scaryClownSpawner = GameObject.FindGameObjectWithTag("ClownStart").GetComponent<SpawnClownAI>();

        audioSource = GetComponentInChildren<AudioSource>();
        destination = transform.position;
        //nav agent 
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = clownSpeed;

        //pour l'animateur vitesse
        animator = GetComponent<Animator>();
        animatorVitesseHash = Animator.StringToHash(SPEED);
        //menacing
        animatorMenacingHash = Animator.StringToHash(MENACING);

        //trouve tous les players 
        //GameObject.FindGameObjectsWithTag("Player").ToList().ForEach(player => players.Add(player.GetComponent<NetworkObject>()));
        //ensuite set la destination au player le plus proche
        FindClosestPlayer();



        base.OnNetworkSpawn();
    }

    [Rpc(SendTo.Server)]
    public void ChangeMenacingLookRpc()
    {
        lookingMenacing.Value = true;
       // animator.SetBool(animatorMenacingHash, true);
    }

     void FixedUpdate()
    {
        if(lookingMenacing.Value)
            return;
        
        

        //action du clown quand actif
        //trouve continuellement le player le plus proche
        FindClosestPlayer();

        //si le clown a trouvé un player
        if( destination != transform.position && navAgent!=null)
        {
            navAgent.SetDestination(destination);

            //ajuste l'animation selon la vitesse actuelle du clown
            float currentSpeed = navAgent.velocity.magnitude;
            animator.SetFloat(animatorVitesseHash, currentSpeed);

            if (audioSource != null && currentSpeed > 1 && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
            
            if(audioSource != null &&  currentSpeed <1 && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
        //Debug.Log(navAgent.velocity.magnitude);

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("clown collide ");

        if (other.tag == "Player")
        {
            Debug.Log("clown collide avec  le player");
            PlayerController cont = other.GetComponent<PlayerController>();
            cont.KillPlayer();
            scaryClownSpawner.ChangeClownActivityFalseRpc();
            DespawnRpcServerRpc();
        }

        if (other.tag == "ClownStop")
        {
            scaryClownSpawner.ChangeClownActivityFalseRpc();
            DespawnRpcServerRpc();

        }
    }

    //trouver le player le plus proche et set la destination du clown sinon destination sera à Zero
    private void FindClosestPlayer()
    {
        if (!IsServer)
            return ;

        //id player plus proche du clown
        ulong playerid = 0;
        //ne part pas après les player si il sont plus de 40 de distance
        float minDistance = 1;
        float distanceTmp = 0;

        foreach (var player in NetworkManager.Singleton.ConnectedClients)
        {
            if (player.Value.PlayerObject == null) continue;

            distanceTmp = Vector3.Distance(player.Value.PlayerObject.transform.position, transform.position);
            if (distanceTmp <= minDistance && player.Value.PlayerObject.GetComponent<PlayerController>().isDead.Value != true)
            {
                minDistance = distanceTmp;
                playerid = player.Key;
            }
        }


        Vector3 closestPlayerPosition = transform.position;
        NetworkClient networkClient = null;
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(playerid, out networkClient) && networkClient != null)
        {
            destination = networkClient.PlayerObject.transform.position;
        }
        else
        {
            // If no living player is found, fallback to clown's position
            destination = transform.position;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnRpcServerRpc()
    {
        try
        {
            var instanceNetworkObject = gameObject.GetComponent<NetworkObject>();
            instanceNetworkObject.Despawn();
        }
        catch
        {
            Debug.Log("erreur despawn clown");
        }
    }

}
