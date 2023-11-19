using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexTrigger : MonoBehaviour
{
    private HingeJoint hinge;

    private void Awake()
    {
        hinge = GetComponent<HingeJoint>();
    }

    private void OnTriggerEnter(Collider other)
    {
        hinge.connectedBody = other.gameObject.GetComponent<Rigidbody>();
    }

    private void OnTriggerExit(Collider other)
    {
        hinge.connectedBody = null;
    }
}
