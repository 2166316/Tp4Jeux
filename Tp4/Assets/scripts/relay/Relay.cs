using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Collections;
using UnityEngine.UI;
using Unity.Networking.Transport.Relay;
using TMPro;

public class Relay : NetworkBehaviour
{
    [SerializeField] private TMPro.TMP_Text textVal;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private int nbPlayerMax = 4;
    [SerializeField] private NetworkVariable<FixedString4096Bytes> codeConnexion = new NetworkVariable<FixedString4096Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();

        // Check if the player is already signed in
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            // Callback
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Player with ID: " + AuthenticationService.Instance.PlayerId);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        else
        {
            Debug.Log("Player already signed in with ID: " + AuthenticationService.Instance.PlayerId);
        }

        QualitySettings.vSyncCount = 0;

        loginButton.onClick.AddListener(() =>
        {
            Debug.Log(textVal.text);
            JoinRelay(textVal.text);
        });

        hostButton.onClick.AddListener(() =>
        {
            CreateRelay();
        });
    }


    private void Update()
    {
        
    }

    public string getCodeConnexion()
    {
        string code = codeConnexion.Value.ToString();
        return code;
    }

    [Rpc(SendTo.Server)]
    public void ChangeCodeRpc(string connexionCode) {
        codeConnexion.Value = connexionCode;
        
    }

    //création du relay et du host
    public async void CreateRelay()
    {
        try
        {
            //nombre de player
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(nbPlayerMax);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            ChangeCodeRpc(joinCode);
            Debug.Log("Code pour rejoindre: "+joinCode);

            GameObject codeText = GameObject.FindWithTag("RelayCode");
            if (codeText != null)
            {
                Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAa");
                TextMeshProUGUI textComponent = codeText.GetComponent<TextMeshProUGUI>();
                textComponent.text = "Code : " + joinCode;
            } else {
                Debug.Log("CODE UI ELEMENT NOT FOUND ");
            }

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

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
            Debug.Log("Client join avec le code: " + joinCode);

            //si on lit le texte d'un TMpro ca rajoute un blankspace donc l'enlever sinon erreur 400
            joinCode = joinCode.Substring(0, 6);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation,"dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError(ex);
        }
    }
}
