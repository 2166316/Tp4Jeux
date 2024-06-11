using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class SpawnClownAI : NetworkBehaviour
{
    [SerializeField] private GameObject clownAIPrefab;

    private List<Vector3> listDePositionPredefiniePourClown;
    private GameObject clownAINetworkObjectRef = null;
   // private NetworkVariable<GameObject> clownAINetworkObjectRef = new NetworkVariable<GameObject>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private NetworkVariable<bool> clownIsActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private List<Vector3> listDePositionPredefiniePourClownIdle;

    public override void OnNetworkSpawn()
    {
        listDePositionPredefiniePourClown = new List<Vector3>
        {
            new Vector3(9.5f, 68f,-149f),
            new Vector3(-19f, 74f,-137.3f),
            new Vector3(29f, 68f,-120f),
            new Vector3(39f, 74f,-145f),
        };

        listDePositionPredefiniePourClownIdle = new List<Vector3>
        {
            new Vector3(Random.Range(1f,16f), 74f,-133.5f),
        };
        base.OnNetworkSpawn();
    }

    [Rpc(SendTo.Server)]
    public void ChangeClownActivityTrueRpc()
    {
        clownIsActive.Value = true;
    }

    [Rpc(SendTo.Server)]
    public void ChangeClownActivityFalseRpc()
    {
        clownIsActive.Value = false;
    }

    [ServerRpc(RequireOwnership =false)]
    private void InstantieClownIdleServerRpc()
    {


        int position = Random.Range(0, listDePositionPredefiniePourClownIdle.Count);

        clownAINetworkObjectRef = Instantiate(clownAIPrefab, listDePositionPredefiniePourClownIdle[position], new Quaternion(0f, 180f, 0f, 0f));

        //met le clown � actif
        ScaryClownController controllerClown = clownAINetworkObjectRef.GetComponent<ScaryClownController>();
        //pour indiquer au spawner de pas en faire spawner d'autre
        ChangeClownActivityTrueRpc();
        NetworkObject networkObject = clownAINetworkObjectRef.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
            //pour indiquer au clown qu'il est idle
            controllerClown.ChangeMenacingLookRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InstantieClownActiveServerRpc()
    {


        int position = Random.Range(0, listDePositionPredefiniePourClown.Count);

        clownAINetworkObjectRef =  Instantiate(clownAIPrefab, listDePositionPredefiniePourClown[position], new Quaternion(0f, 0f, 0f, 0f));

        //met le clown  actif
        ScaryClownController controllerClown = clownAINetworkObjectRef.GetComponent<ScaryClownController>();
        //controllerClown.ChangeClownActivityRpc();
        NetworkObject networkObject = clownAINetworkObjectRef.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
           
            if (!clownIsActive.Value)
            {

                    StartCoroutine(SpawnClownAtRandomMoment());
                    ChangeClownActivityTrueRpc();
            
 
            }     
        }

        if (other.tag == "Clown")
        {

                ChangeClownActivityFalseRpc();
                


        }
    }

    private  IEnumerator SpawnClownAtRandomMoment()
    {

        InstantieClownIdleServerRpc();

        // Wait for the clown to spawn and initialize clownAINetworkObjectRef
       // yield return new WaitUntil(() => clownAINetworkObjectRef != null);

        int tempEnSecondes = Random.Range(2, 4);
        yield return new WaitForSeconds(tempEnSecondes);
        Debug.Log("test1");
        ScaryClownController scaryClownController = GameObject.FindGameObjectWithTag("Clown").GetComponent<ScaryClownController>();
        if (scaryClownController != null)
        {
            scaryClownController.DespawnRpcServerRpc();
        }
        Debug.Log("test2");
        tempEnSecondes = Random.Range(2, 5);
        yield return new WaitForSeconds(tempEnSecondes);
        InstantieClownActiveServerRpc();
    }


}
