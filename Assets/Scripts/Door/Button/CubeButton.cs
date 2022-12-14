using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CubeButton : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;

    private AllButtonPressed parent;
    private bool isPressed;
    private int objectsOnButton;

    public void SetParent(AllButtonPressed parent)
    {
        this.parent = parent;
    }

    public bool IsPressed() { return isPressed; }

    private void OnTriggerEnter(Collider other)
    {
        if (layerMask == (layerMask | 1 << other.gameObject.layer))
        {
            objectsOnButton++;
            //Debug.Log(objectsOnButton);
            isPressed = true;
            parent.IsAllPressed();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (layerMask == (layerMask | 1 << other.gameObject.layer))
        {
            objectsOnButton--;
            //Debug.Log(objectsOnButton);
            if (objectsOnButton == 0)
            {
                isPressed = false;
                parent.IsAllPressed();
            }
        }
    }
}
