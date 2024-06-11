using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class KeySpawnerController : NetworkBehaviour
{
    private NetworkVariable<int> numberOfKeysPickedUp = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private List<Vector3> houseSpawnPointsEasy;
    private List<Vector3> houseSpawnPointsHard;
    private List<Vector3> outsideSpawnPoints;

    [SerializeField] private GameObject clePrefab;


    public override void OnNetworkSpawn()
    {
        

        //les positions
        houseSpawnPointsEasy = new List<Vector3>
        {
            new Vector3(-17f,68f,-143f),
            //new Vector3(-20.3f,67.5f,-134f), 
            new Vector3(31.85f,68f,-117.5f),
        };

        houseSpawnPointsHard = new List<Vector3>
        {
            new Vector3(40.9f,73.3f,-113.1f),
            new Vector3(-19.8f,74f,-146.8f),
            new Vector3(8.85f,73.25f,-152.6f),
        };

        outsideSpawnPoints = new List<Vector3>
        {
            new Vector3(-39f,63.85f,-131.5f),
            new Vector3(-39f,63.85f,-66f),
            new Vector3(15.53f,63.33f,-58.29f),
            new Vector3(-9.32f,64.06f,-84.23f),
            new Vector3(-22.55f,62.93f,-102.58f),
            new Vector3(-14.29f,62.93f,-91.34f),
            new Vector3(3.58f,62.93f,-72.73f),
            new Vector3(6.17f,62.93f,-90.66f),
            new Vector3(6.5f,63.2f,-121.44f),
            new Vector3(31f,62.93f,-52.46f),
            new Vector3(-16.7f,63.13f,-67.58f),
            new Vector3(-28.21f,63.13f,-63.4f),
        };

        if (!IsHost)
            return;

        //spawn
        SpawnDecider();

        base.OnNetworkSpawn();
    }

   private void SpawnItems(List<Vector3> positions)
    {
        foreach (Vector3 pos in positions)
        {
            GameObject cle = Instantiate(clePrefab, pos, new Quaternion(0f, 0f, 0f, 0f));
            NetworkObject networkObject = cle.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Spawn();
            }
        }
    }

    public int GetKeysPickedUp()
    {
        return numberOfKeysPickedUp.Value;
    }

    [Rpc(SendTo.Server)]
    public void PickUpKeyRpc()
    {
        numberOfKeysPickedUp.Value += 1;
        Debug.Log(numberOfKeysPickedUp.Value);
    }

    private void SpawnDecider()
    {
        List<Vector3> positions = new();

        int randomLayout = Random.Range(0, 3);
        Debug.Log("Game type: " + randomLayout);
        switch(randomLayout)
        {
            //cl� maison 2 et dehors 4
            case 0:
                positions.Add(houseSpawnPointsEasy[Random.Range(0, houseSpawnPointsEasy.Count)]);
                positions.Add(houseSpawnPointsHard[Random.Range(0, houseSpawnPointsHard.Count)]);
                for (int i = 0; i < 4; i++)
                {
                    Vector3 newPosition = outsideSpawnPoints[Random.Range(0, outsideSpawnPoints.Count)];
                    while (positions.Contains(newPosition))
                    {
                        newPosition = outsideSpawnPoints[Random.Range(0, outsideSpawnPoints.Count)];
                    }
                    positions.Add(newPosition);
                }
                break;
            //cl� maison 3 et dehors 3
            case 1:
                positions.Add(houseSpawnPointsEasy[Random.Range(0, houseSpawnPointsEasy.Count)]);

                for (int i = 0; i < 2; i++)
                {
                    Vector3 newPosition = houseSpawnPointsHard[Random.Range(0, houseSpawnPointsHard.Count)];
                    while (positions.Contains(newPosition))
                    {
                        newPosition = houseSpawnPointsHard[Random.Range(0, houseSpawnPointsHard.Count)];
                    }
                    positions.Add(newPosition);
                }

                for (int i = 0; i < 3; i++)
                {
                    Vector3 newPosition = outsideSpawnPoints[Random.Range(0, outsideSpawnPoints.Count)];
                    while (positions.Contains(newPosition))
                    {
                        newPosition = outsideSpawnPoints[Random.Range(0, outsideSpawnPoints.Count)];
                    }
                    positions.Add(newPosition);
                }
                break;
            //cl� maison 1 et dehors 5
            case 2:
                positions.Add(houseSpawnPointsHard[Random.Range(0, houseSpawnPointsHard.Count)]);

                for (int i = 0; i < 5; i++)
                {
                    Vector3 newPosition = outsideSpawnPoints[Random.Range(0, outsideSpawnPoints.Count)];
                    while (positions.Contains(newPosition))
                    {
                        newPosition = outsideSpawnPoints[Random.Range(0, outsideSpawnPoints.Count)];
                    }
                    positions.Add(newPosition);
                }
                break;
            //cl� maison 2 et dehors 4
            default: 
                break;
        }

        //test positions  TO_comment
        /*positions.Add(new Vector3(7.7f,63.7f,-167.8f));
        positions.Add(new Vector3(8.3f, 63.7f, -167.8f));
        positions.Add(new Vector3(7.4f, 63.7f, -167.8f));
        positions.Add(new Vector3(8.6f, 63.7f, -167.8f));
        positions.Add(new Vector3(7.0f, 63.7f, -167.8f));
        positions.Add(new Vector3(8.9f, 63.7f, -167.8f));*/

        SpawnItems(positions);
    }
}
