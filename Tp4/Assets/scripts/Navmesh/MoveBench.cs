using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBench : MonoBehaviour
{
    public Transform targetObject;
    public float rotationSpeed = 5f;

    private void Update()
    {
        // Get the position of the target object
        Vector3 targetPosition = targetObject.position;

        // Rotate around the target object's position on the y-axis
        transform.RotateAround(targetPosition, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
