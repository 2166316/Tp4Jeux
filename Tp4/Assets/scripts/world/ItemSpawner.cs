using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
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
            new Vector3(-20.3f,67.5f,-134f), 
            new Vector3(31.85f,68f,-117.5f),
        };

        houseSpawnPointsHard = new List<Vector3>
        {
            new Vector3(29.5f,74.3f,-117.05f),
            new Vector3(-15f,74f,-137.3f),
            new Vector3(10.5f,73.25f,-152.6f),
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


    private void SpawnDecider()
    {
        List<Vector3> positions = new();

        int randomLayout = Random.Range(0, 3);
        Debug.Log("Game type: " + randomLayout);
        switch(randomLayout)
        {
            //clé maison 2 et dehors 4
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
            //clé maison 3 et dehors 3
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
            //clé maison 1 et dehors 5
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
            //clé maison 2 et dehors 4
            default: 
                break;
        }
        SpawnItems(positions);
    }
}
