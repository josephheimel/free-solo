using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(Mesh))]
public class MakePolygonal : MonoBehaviour
{
    [SerializeField] 
    private Mesh mesh;

    void Start()
    {
        GetComponent<MeshFilter>().mesh = mesh;

    }
}
