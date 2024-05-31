using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DoorController : NetworkBehaviour
{
    private KeySpawnerController keyController;

    // Start is called before the first frame update
    void Start()
    {
        keyController = FindAnyObjectByType<KeySpawnerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (keyController.GetKeysPickedUp().ToString() == "6")
        {
            DespawnDoorRpc();
        }
    }

    [Rpc(SendTo.Server)]
    public void DespawnDoorRpc()
    {
        if (IsServer)
        {
            NetworkObject thisObj = gameObject.GetComponent<NetworkObject>();
            thisObj.Despawn();
        }

    }
}
