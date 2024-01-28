using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGravityController : MonoBehaviour
{
    [SerializeField]
    private ConstantForce cf;

    public GameObject gravityPoint;
    public float rotationSpeed = 20;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cf = GetComponent<ConstantForce>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gravityPoint)
        {
            Vector3 gravityUp = (transform.position - gravityPoint.transform.position).normalized;
            cf.force = (-gravityUp * gravityPoint.GetComponent<GravityPoint>().gravity) * rb.mass;
        }
    }
}
