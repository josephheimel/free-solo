using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class CreateVertexPoints : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    private void Start()
    {
        GetChildren(transform);
    }

    private void GetChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.GetComponent<MeshFilter>() != null)
            {
                foreach (Vector3 v in child.GetComponent<MeshFilter>().mesh.vertices)
                {
                    Vector3 worldPoint = child.TransformPoint(v);
                    Instantiate(prefab, worldPoint, Quaternion.identity, child);
                }
            }

            if (child.childCount > 0)
            {
                GetChildren(child);
            }
        }
    }
}

/**
 * TODO:
 *
 * BUGS:
 *
*/