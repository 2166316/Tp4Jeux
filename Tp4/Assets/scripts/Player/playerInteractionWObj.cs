using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class playerInteractionWObj : NetworkBehaviour
{
    public Material highlightMaterial;
    private Material originalMaterial;
    private GameObject lastHighlightedObject;
    private float interactionDistance = 4.0f;
    public Animator animator;
    private float pickupAnimationTime = 0;
    private float animationTimer = 2.5f;
    private bool animTime = false;

    private KeySpawnerController keyController;

    private Camera playerCamera;
    public override void OnNetworkSpawn()
    {
        playerCamera = GetComponentInChildren<Camera>();
        keyController = FindAnyObjectByType<KeySpawnerController>();
        base.OnNetworkSpawn();
    }

    void HighlightObject(GameObject gameObject)
    {
        if (lastHighlightedObject != gameObject && gameObject.CompareTag("Object"))
        {
            ClearHighlighted();
            originalMaterial = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
            gameObject.GetComponent<MeshRenderer>().sharedMaterial = highlightMaterial;
            lastHighlightedObject = gameObject;
        }
    }

    void ClearHighlighted()
    {
        if (lastHighlightedObject != null)
        {
            lastHighlightedObject.GetComponent<MeshRenderer>().sharedMaterial = originalMaterial;
            lastHighlightedObject = null;
        }
    }

    void HighlightObjectInCenterOfCam()
    {
        float rayDistance = interactionDistance;
        // Ray from the center of the viewport.
        if (playerCamera != null)
        {

        }
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit rayHit;
        // Check if we hit something.
        if (Physics.Raycast(ray, out rayHit, rayDistance))
        {
            // Get the object that was hit.
            GameObject hitObject = rayHit.collider.gameObject;
            HighlightObject(hitObject);
            // Delete gameObject when pressing E + other actions
            if (Input.GetKeyDown(KeyCode.E) && hitObject.CompareTag("Object"))
            {


                animator.SetBool("pickup", true);
                StartCoroutine(waitOne());

                if (!IsOwner) return;
                KeyBehavior cleNetWork = hitObject.GetComponent<KeyBehavior>();
                cleNetWork.DespawnKeyRpc();

                keyController.PickUpKeyRpc();
            }
        }
        else
        {
            ClearHighlighted();
        }
    }

    private IEnumerator waitOne()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("pickup", false);
    }

    void Update()
    {
        HighlightObjectInCenterOfCam();

    }
}
