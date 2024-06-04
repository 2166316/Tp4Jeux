using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenuScript : NetworkBehaviour
{

    private GameObject panel;
    [SerializeField]private Button disconnectButton;
    private Relay relay;
    private void Start()
    {
        
        //menu
        panel = GameObject.FindGameObjectWithTag("InGameMenu");

        if(disconnectButton != null)
        {
            disconnectButton.onClick.AddListener(() =>
            {
                Debug.Log("test1");
                DisconnectServerRpc();
            });
        }

        if (panel != null)
        {
            panel.SetActive(false);
        }
        else
        {
            Debug.LogError("error ingamemenu");
        }
    }

    public override void OnNetworkSpawn()
    {
        relay = FindAnyObjectByType<Relay>();


        base.OnNetworkSpawn();
    }

    void Update()
    {
        //active le menu
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMenu();
            //get relay code
            if (!Equals(relay, null))
            {
                string code = relay.getCodeConnexion();
                /*if (code != null)
                {
                    Debug.Log("THE CODE IS :: " + code);
                } else
                {
                    Debug.Log("ERROR: NOT ABLE TO FETCH CONNEXION CODE");
                }*/
            }
        }
    }

    void ToggleMenu()
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);

            // Toggle cursor visibility based on the panel's active state
            if (panel.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DisconnectServerRpc(ServerRpcParams serverRpcParams = default)
    {
        try
        {
            var clientId = serverRpcParams.Receive.SenderClientId;
            Debug.Log("id : " + clientId);

            NetworkObject client = NetworkManager.Singleton.ConnectedClients.Values.ToList().FirstOrDefault(n => n.ClientId == clientId).PlayerObject;
            if (client != null)
            {
                NetworkManager.Singleton.DisconnectClient(clientId);
                client.Despawn();
            }
        }
        catch (Exception e) { 
           // Debug.LogException(e);
        }
    }

    
}
