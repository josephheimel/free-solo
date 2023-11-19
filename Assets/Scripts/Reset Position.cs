using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    [SerializeField]
    private Vector3 initialPosition;
    [SerializeField]
    private Quaternion initialRotation;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Reset()
    {
        rb.isKinematic = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Reset();
        }

        if (rb.isKinematic)
        {
            if (transform.position != initialPosition)
            {
                transform.position = Vector3.Slerp(transform.position, initialPosition, Time.deltaTime * 10);
            }

            /*
            if (transform.rotation != initialRotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, Time.deltaTime);
            }

            && transform.rotation == initialRotation
            */

            if (transform.position == initialPosition)
            {
                rb.isKinematic = false;
            }
        }
    }
}

/**
 * TODO:
 *
 * BUGS:
 */