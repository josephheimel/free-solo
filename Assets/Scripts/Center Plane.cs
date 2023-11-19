using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterPlane : MonoBehaviour
{
    [SerializeField]
    private Transform ringTransform;

    void Update()
    {
        transform.position = ringTransform.position;
    }
}

/**
 * TODO:
 *
 * BUGS:
 *
*/