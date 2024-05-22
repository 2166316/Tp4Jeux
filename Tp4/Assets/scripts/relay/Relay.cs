using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Collections;

public class Relay : NetworkBehaviour
{
    private int nbPlayerMax = 4;
    private NetworkVariable<FixedString4096Bytes> codeConnexion = new();

    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();

        //callback
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Player signed in :" + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            
            CreateRelay();
        }

       /* if (Input.GetKeyDown(KeyCode.C))
        {
           
            JoinRelay("");
        }*/
    }

    [Rpc(SendTo.Server)]
    public void ChangeCodeRpc(string connexionCode) {
        codeConnexion.Value = connexionCode;
    }


    //cr�ation du relay et du host
    public async void CreateRelay()
    {
        try
        {
            //nombre de player
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(nbPlayerMax);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            ChangeCodeRpc(joinCode);
            Debug.Log("Code pour rejoindre: "+joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData

                );
            NetworkManager.Singleton.StartHost();
        }
        catch(RelayServiceException ex)
        {
            Debug.LogError(ex);
        }
    }

    //partir un client
    public async void JoinRelay(string joinCode)
    {
        try
        {
            //temporairement automatique
            joinCode = codeConnexion.Value.ToString();
            Debug.Log("Client join avec le code: " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                    joinAllocation.RelayServer.IpV4,
                    (ushort)joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.Key,
                    joinAllocation.ConnectionData,
                    joinAllocation.HostConnectionData
                );

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError(ex);
        }
    }
}
