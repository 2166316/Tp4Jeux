using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ScaryClownController : MonoBehaviour
{
    private GameObject start;
    private Vector3 startPosition;
    private bool isMoving;
    private UnityEngine.AI.NavMeshAgent navAgent;

    private Animator animator;
    private const string SPEED = "Speed";
    private int animatorVitesseHash;

    [SerializeField] public GameObject destination;

    void Start()
    {
        startPosition = new Vector3(5, 66, 138);

        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();


        navAgent.speed = 3;

        animator = GetComponent<Animator>();

        animatorVitesseHash = Animator.StringToHash(SPEED);
        isMoving = true;
        StartCoroutine(LookMenacingWait());
    }


    void Update()
    {

        if (!isMoving)
        {
            navAgent.SetDestination(destination.transform.position);
            isMoving = true;
        }
        Debug.Log(navAgent.velocity.magnitude);
        float currentSpeed = navAgent.velocity.magnitude;
        animator.SetFloat(animatorVitesseHash, currentSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
    }

    private IEnumerator LookMenacingWait(){
        yield return new WaitForSeconds(5);
        
        Debug.Log(animator.speed);
        isMoving = false;
    }


}
