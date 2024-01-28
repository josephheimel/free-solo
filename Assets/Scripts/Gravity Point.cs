using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GravityPoint : MonoBehaviour
{
    // serialized

    public float gravity;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<LocalGravityController>())
        {
            other.GetComponent<LocalGravityController>().gravityPoint = transform.gameObject;
        }
    }
}