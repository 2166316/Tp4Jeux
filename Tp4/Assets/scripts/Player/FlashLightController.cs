using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

public class FlashLightController : NetworkBehaviour
{
    private float sensitivity = 100f;
    private float rotationY = 0f;
    private Light lightSpot;

    // Start is called before the first frame update
    void Start()
    {
        lightSpot = GetComponentInChildren<Light>();
        lightSpot.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        rotationY += mouseY;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);
        transform.localRotation = Quaternion.Euler(-rotationY, 0f, 0f);

        if (Input.GetKeyDown(KeyCode.F)) {

            lightSpot.enabled = !lightSpot.enabled;
        }
    }
}
