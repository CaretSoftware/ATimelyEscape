using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{

    [SerializeField] private Camera thirdPersonCamera;
    [SerializeField] private Camera firstPersonCamera;
    void Start()
    { 
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift)){
            thirdPersonCamera.enabled = false;
            firstPersonCamera.enabled = true;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift)){
            firstPersonCamera.enabled = false;
            thirdPersonCamera.enabled = true;

        }
   
    }
}
