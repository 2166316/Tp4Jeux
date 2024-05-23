using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private int clownSpeed =5;

    private Vector3 destination;

    private AudioSource audioSource;

    private List<NetworkObject> players = new();

    private NetworkVariable<bool> clownIsActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    [Rpc(SendTo.Server)]
    public void ChangeClownActivityRpc()
    {
        clownIsActive.Value = !clownIsActive.Value;
    }

    public override void OnNetworkSpawn()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        destination = transform.position;
        //nav agent 
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = clownSpeed;

        //pour l'animateur
        animator = GetComponent<Animator>();
        animatorVitesseHash = Animator.StringToHash(SPEED);

        //trouve tous les players 
        GameObject.FindGameObjectsWithTag("Player").ToList().ForEach(poubelle => players.Add(poubelle.GetComponent<NetworkObject>()));
        //ensuite set la destination au player le plus proche
        FindClosestPlayer();

        base.OnNetworkSpawn();
    }

     void FixedUpdate()
    {
        //trouve continuellement le player le plus proche
        FindClosestPlayer();

        //si le clown a trouvé un player
        if( destination != transform.position)
        {
            navAgent.SetDestination(destination);
        }
        //Debug.Log(navAgent.velocity.magnitude);

        //ajuste l'animation selon la vitesse actuelle du clown
        float currentSpeed = navAgent.velocity.magnitude;
        animator.SetFloat(animatorVitesseHash, currentSpeed);

        if(audioSource != null && currentSpeed > 1 && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerController cont =other.GetComponent<PlayerController>();
            cont.KillPlayerRpc();
            DespawnRpc();
        }

        if (other.tag == "ClownStart")
        {
            DespawnRpc();
        }
    }

    //trouver le player le plus proche et set la destination du clown sinon destination sera à Zero
    private void FindClosestPlayer()
    {
        if (!IsServer)
            return ;

        //id player plus proche du clown
        ulong playerid = 0;
        //ne part pas après les player si il sont plus de 10 de distance
        float minDistance = 10;

        foreach (var player in NetworkManager.Singleton.ConnectedClients)
        {
            //skip le reste si null
            if (player.Value.PlayerObject == null) continue;

            float distanceTmp = 0;
            if((distanceTmp = Vector3.Distance(player.Value.PlayerObject.transform.position, this.transform.position)) <= minDistance)
            {
                minDistance = distanceTmp;
                playerid = player.Key;
            }
        }

        NetworkClient networkClient = null;
        NetworkManager.Singleton.ConnectedClients.TryGetValue(playerid, out networkClient);

        Vector3 closestPlayerPosition = transform.position;
        if(networkClient != null)
        {
            closestPlayerPosition = networkClient.PlayerObject.transform.position;  
        }

        destination = closestPlayerPosition;
        
    }

    private IEnumerator LookMenacingWait(){
        yield return new WaitForSeconds(5);
        
        Debug.Log(animator.speed);
    }

    [Rpc(SendTo.Server)]
    public void DespawnRpc()
    {
        try
        {
            var instanceNetworkObject = gameObject.GetComponent<NetworkObject>();
            instanceNetworkObject.Despawn();
        }
        catch
        {
        }
    }

}
