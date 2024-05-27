using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DisconnectController : NetworkBehaviour
{
    public void LoadScene(string name)
    {
        try
        {
            DisconnectServerRpc();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            LoadScene("");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DisconnectServerRpc(ServerRpcParams serverRpcParams = default)
    {

        var clientId = serverRpcParams.Receive.SenderClientId;
        Debug.Log("id : " + clientId);
        NetworkObject clientPickup = NetworkManager.Singleton.ConnectedClients.Values.ToList().FirstOrDefault(n => n.ClientId == clientId).PlayerObject;
        if (clientPickup != null)
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
            // clientPickup.Despawn();
        }
    }
}
