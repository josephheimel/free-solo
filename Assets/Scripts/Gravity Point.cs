using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GravityPoint : MonoBehaviour
{
    // public
    public float gripGravity;
    public float jumpGravity;

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.GetComponent<TouchManager>())
        {
            collision.gameObject.GetComponent<TouchManager>().gripGravityPoint = transform.gameObject;
        }
    }
}