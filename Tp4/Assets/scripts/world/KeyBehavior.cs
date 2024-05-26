using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KeyBehavior : NetworkBehaviour
{
    [Rpc(SendTo.Server)]
    public void DespawnKeyRpc()
    {
        if (IsServer)
        {
            NetworkObject thisObj = gameObject.GetComponent<NetworkObject>();
            thisObj.Despawn();
        }

    }
}
