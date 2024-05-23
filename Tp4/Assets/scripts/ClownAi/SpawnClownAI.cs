using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class SpawnClownAI : NetworkBehaviour
{
    [SerializeField] private GameObject clownAIPrefab;

    private List<Vector3> listDePositionPredefiniePourClown;
    private GameObject clownAINetworkObjectRef;

    public override void OnNetworkSpawn()
    {
        listDePositionPredefiniePourClown = new List<Vector3>
        {
            new Vector3(9.5f, 68f,-149f),
            new Vector3(-19f, 74f,-137.3f),
            new Vector3(29f, 68f,-120f),
            new Vector3(39f, 74f,-145f),

        };
        base.OnNetworkSpawn();
    }

    private void InstantieClown()
    {
        int position = Random.Range(0, listDePositionPredefiniePourClown.Count);

        clownAINetworkObjectRef = Instantiate(clownAIPrefab, listDePositionPredefiniePourClown[position], new Quaternion(0f, 0f, 0f, 0f));

        //met le clown à actif
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
        if(other.tag == "Player")
        {
            if (object.Equals(clownAINetworkObjectRef, null)){
                StartCoroutine(spawnClownAtRandomMoment());
            }     
        }
    }

    private IEnumerator spawnClownAtRandomMoment()
    {
        int tempEnSecondes = Random.Range(1, 5);

        yield return new WaitForSeconds(tempEnSecondes);
        InstantieClown();
    }


}
