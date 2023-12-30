using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ring")
        {
            other.gameObject.AddComponent<HingeJoint>().connectedBody = transform.gameObject.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Destroy(other.gameObject.GetComponent<HingeJoint>());
    }
}
