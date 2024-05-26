using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ScoreHolder : NetworkBehaviour
{

    [SerializeField] private TextMeshProUGUI score;
    private NetworkVariable<FixedString4096Bytes> scoreText = new NetworkVariable<FixedString4096Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private KeySpawnerController keyController;
    public override void OnNetworkSpawn()
    {
        keyController = FindAnyObjectByType<KeySpawnerController>();


        base.OnNetworkSpawn();
    }

    void Update()
    {

        score.text = scoreText.Value.ToString();
        changeText();
    }

    public void changeText()
    {
        if (!IsServer) return;

        if (!Equals(keyController,null))
        {
            if (keyController.GetKeysPickedUp() >= 6)
            {
                scoreText.Value = "Escape!";
            }
            else
            {
                scoreText.Value = "Keys: " + keyController.GetKeysPickedUp().ToString();
            }
        }
    }
}
