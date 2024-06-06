using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBench : MonoBehaviour
{
    public Transform targetObject;
    private float rotationSpeed = 40f;

    private void Update()
    {
        Vector3 targetPosition = targetObject.position;

        transform.RotateAround(targetPosition, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
