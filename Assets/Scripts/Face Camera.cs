using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(mainCamera.transform.position), Time.deltaTime);
    }
}

/**
 * TODO:
 * - ring prefab
 *
 * BUGS:
 *
*/