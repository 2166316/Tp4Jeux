using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class playerInteractionWObj : NetworkBehaviour
{
    public Material highlightMaterial;
    private Material originalMaterial;
    private GameObject lastHighlightedObject;
    public Animator animator;
    private float interactionDistance = 3.0f;
    private float pickuptimerLimit = 1.8f;
    private float pickupTimer = 0f;
    private bool itempickup = false;

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
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, rayDistance))
        {
            GameObject hitObject = rayHit.collider.gameObject;
            HighlightObject(hitObject);
            if (Input.GetKeyDown(KeyCode.E) && hitObject.CompareTag("Object"))
            {
                itempickup = true;
                animator.SetBool("pickup", true);
                Destroy(hitObject);
            }
        }
        else
        {
            ClearHighlighted();
        }
    }

    void Update()
    {
        HighlightObjectInCenterOfCam();
        if (itempickup)
        {
            pickupTimer += Time.deltaTime;
            if (pickupTimer > pickuptimerLimit)
            {
                itempickup = false;
                animator.SetBool("pickup", false);
                pickupTimer = 0;
            }
        }
    }
}
