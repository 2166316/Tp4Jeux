using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : NetworkBehaviour
{
    public static MonsterController instance;
    public Animator animator;
    public AnimationClip animationClip;
    public AudioSource audioSource;
    public AudioClip normalAudioClip; // Audio clip for normal movement
    public AudioClip bitingAudioClip; // Audio clip for biting

    private float minSpeed = 2f;
    private float maxSpeed = 5f;
    private float currentSpeed = 0f;
    private float speedIncrement = 0.2f;

    public NavMeshAgent agent;
    private float wanderTimer = 15f;
    private float timer;
    private NavMeshTriangulation navMeshData;

    public bool aggroed = false;
    private bool biting = false;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        timer = wanderTimer;
        navMeshData = NavMesh.CalculateTriangulation();

        agent.speed = minSpeed;
        agent.acceleration *= 20f;
        agent.angularSpeed = 360f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return; // Ensure this runs only on the server in a networked environment

        if (aggroed && !biting)
        {
            IncreaseSpeed();
        }
        else if (!aggroed && !biting)
        {
            DecreaseSpeed();
        }

        timer += Time.deltaTime;

        if (timer >= wanderTimer && !biting)
        {
            Vector3 newPos = GetRandomPointOnNavMesh();
            agent.SetDestination(newPos);
            timer = 0;
        }

        animator.SetFloat("speed", currentSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !biting)
        {
            audioSource.Stop();
            PlayerController cont = other.GetComponent<PlayerController>();
            cont.KillPlayerRpc();
            MonsterAggro.instance.playerDead = true;
            StartCoroutine(BiteRoutine());
        }
    }

    private IEnumerator BiteRoutine()
    {
        agent.isStopped = true;  // Stop the NavMeshAgent
        biting = true;
        animator.SetBool("biting", true);
        agent.velocity = Vector3.zero;

        audioSource.clip = bitingAudioClip; // Switch to biting audio clip
        audioSource.pitch = 1f; // Reset pitch for biting audio
        audioSource.volume = 0.3f; // Set appropriate volume for biting audio
        audioSource.Play();

        float animationLength = animationClip.length;
        yield return new WaitForSeconds(animationLength * 3);

        timer = wanderTimer + 1f;
        animator.SetBool("biting", false);
        biting = false;
        agent.isStopped = false; // Resume the NavMeshAgent
        audioSource.clip = normalAudioClip;
    }

    private Vector3 GetRandomPointOnNavMesh()
    {
        // Find a random point within the bounds of the NavMesh
        Vector3 randomPoint = new Vector3(
            Random.Range(navMeshData.vertices[0].x, navMeshData.vertices[navMeshData.vertices.Length - 1].x),
            Random.Range(navMeshData.vertices[0].y, navMeshData.vertices[navMeshData.vertices.Length - 1].y),
            Random.Range(navMeshData.vertices[0].z, navMeshData.vertices[navMeshData.vertices.Length - 1].z)
        );

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPoint, out hit, Mathf.Infinity, NavMesh.AllAreas);

        return hit.position;
    }

    private void IncreaseSpeed()
    {
        if (currentSpeed < 1f)
        {
            currentSpeed += speedIncrement * Time.deltaTime; // Increase speed over time
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, 1f); // Clamp the value between 0 and 1
            agent.speed = Mathf.Lerp(minSpeed, maxSpeed, currentSpeed); // Interpolate agent speed
            UpdateAudioProperties(); // Update audio properties based on speed
        }
    }

    private void DecreaseSpeed()
    {
        if (currentSpeed > 0f)
        {
            currentSpeed -= speedIncrement * Time.deltaTime; // Decrease speed over time
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, 1f); // Clamp the value between 0 and 1
            agent.speed = Mathf.Lerp(minSpeed, maxSpeed, currentSpeed); // Interpolate agent speed
            UpdateAudioProperties(); // Update audio properties based on speed
        }
    }

    private void UpdateAudioProperties()
    {
        if (currentSpeed > 0.3f)
        {
            float pitch = Mathf.Lerp(0.3f, 1f, currentSpeed);
            float volume = Mathf.Lerp(0.1f, 0.3f, currentSpeed); // Adjust volume based on speed
            audioSource.pitch = pitch;
            audioSource.volume = volume;

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }
}
