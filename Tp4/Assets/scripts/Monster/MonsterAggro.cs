using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAggro : MonoBehaviour
{
    public static MonsterAggro instance;
    public NavMeshAgent monsterAgent; // Reference to the monster's NavMeshAgent
    private Transform playerTransform; // Reference to the player's transform
    public bool playerDead = false;
    private bool isFollowingPlayer = false; // Flag to check if the monster is following the player

    private float timer;
    private float reaggroTimer = 3f;

    void Start()
    {
        instance = this;
        timer = reaggroTimer;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (playerDead)
        {
            resetMonster();
            playerDead = false;
        }

        // If the monster is following the player, update the destination to the player's position
        if (isFollowingPlayer && playerTransform != null)
        {
            // Find the closest point on the NavMesh to the player
            NavMeshHit hit;
            if (NavMesh.SamplePosition(playerTransform.position, out hit, 1.0f, NavMesh.AllAreas))
            {
                // Set the destination to the closest point on the NavMesh
                monsterAgent.SetDestination(hit.position);
                MonsterController.instance.aggroed = true;
            }
            else
            {
                // If no valid position is found, reset the monster
                resetMonster();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is the player
        if (other.CompareTag("Player"))
        {
            // If the monster is not currently following a player or the re-aggro timer has expired
            if (!isFollowingPlayer || timer >= reaggroTimer)
            {
                playerTransform = other.transform; // Set the player's transform
                isFollowingPlayer = true; // Set the flag to follow the player
                timer = 0; // Reset the re-aggro timer
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object that exited the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Reset the monster only if the exiting player is the current aggroed player
            if (other.transform == playerTransform)
            {
                StartCoroutine(DelayedReset());
            }
        }
    }

    private IEnumerator DelayedReset()
    {
        yield return new WaitForSeconds(2f); // Wait for 2 seconds
        resetMonster();
    }

    private void resetMonster()
    {
        isFollowingPlayer = false; // Unset the flag to follow the player
        playerTransform = null; // Clear the player's transform
        MonsterController.instance.aggroed = false;
    }
}
