using System;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform cameraTarget;
    [SerializeField] private Camera thirdPersonCamera;
    [SerializeField] private Camera exitCamera;
    private const string MainCamera = "MainCamera"; 
    private const string UnTagged = "Untagged";
    private bool zoomedIn;

    void Start()
    {
        NewRatCharacterController.NewCharacterInput.exitZoomDelegate += ControllerZoom;
        exitCamera.enabled = false;
    }

    private void ControllerZoom(bool zoom)
    {
        if (zoom)
            ZoomIn();
        else
            ZoomOut();
    }

    void Update()
    {
        if (zoomedIn)
            ZoomIn();
        return;
    }

    private void ZoomIn()
    {
        zoomedIn = true;
        exitCamera.tag = MainCamera; // To satisfy Fluffy that searches for an active camera with tag "MainCamera" if no active camera assigned
        exitCamera.transform.LookAt(cameraTarget);
        thirdPersonCamera.enabled = false;
        exitCamera.enabled = true;
    }

    private void ZoomOut()
    {
        zoomedIn = false;
        exitCamera.tag = UnTagged;
        exitCamera.enabled = false;
        thirdPersonCamera.enabled = true;
    }

    private void OnDestroy()
    {
        NewRatCharacterController.NewCharacterInput.exitZoomDelegate -= ControllerZoom;
    }
}
