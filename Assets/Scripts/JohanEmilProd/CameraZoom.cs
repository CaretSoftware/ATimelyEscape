using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private Camera thirdPersonCamera;
    [SerializeField] private Camera firstPersonCamera;
    private const string MainCamera = "MainCamera"; 
    private const string UnTagged = "Untagged";

    void Start()
    {
        NewRatCharacterController.NewCharacterInput.zoomDelegate += ControllerExitZoom;
        firstPersonCamera.enabled = false;
    }

    private void ControllerExitZoom(bool zoom)
    {
        if (zoom)
            ZoomIn();
        else
            ZoomOut();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ZoomIn();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ZoomOut();
        }
    }

    private void ZoomIn()
    {
        firstPersonCamera.tag = MainCamera; // To satisfy Fluffy that searches for an active camera with tag "MainCamera" if no active camera assigned
        thirdPersonCamera.enabled = false;
        firstPersonCamera.enabled = true;
    }

    private void ZoomOut()
    {
        firstPersonCamera.tag = UnTagged;
        thirdPersonCamera.enabled = true;
        firstPersonCamera.enabled = false;
    }

    private void OnDestroy()
    {
        NewRatCharacterController.NewCharacterInput.zoomDelegate -= ControllerExitZoom;
    }
}
