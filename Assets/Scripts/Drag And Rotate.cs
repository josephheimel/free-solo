﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DragAndRotate : MonoBehaviour
{
    public bool isActive = false;
    Color activeColor = new Color();

    void Update()
    {
        if (isActive)
        {
            activeColor = Color.red;
            if (Input.touchCount == 1)
            {
                Touch screenTouch = Input.GetTouch(0);
                if (screenTouch.phase == TouchPhase.Moved)
                {
                    
                }
                if (screenTouch.phase == TouchPhase.Ended)
                {
                    isActive = false;
                }
            }
        }
        else
        {
            activeColor = Color.white;
        }
        GetComponent<MeshRenderer>().material.color = activeColor;
    }
}