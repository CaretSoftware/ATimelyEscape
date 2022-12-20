using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour

{

    public Transform cameraTarget;
    // Start is called before the first frame update
    [SerializeField] private Camera thirdPersonCamera;
    [SerializeField] private Camera exitCamera;
    void Start()
    {
        exitCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

            if(Input.GetKeyDown(KeyCode.Tab)){
                transform.LookAt(cameraTarget);
            thirdPersonCamera.enabled = false;
            exitCamera.enabled = true;
        }
        if(Input.GetKeyUp(KeyCode.Tab)){
            exitCamera.enabled = false;
            thirdPersonCamera.enabled = true;
            }
    }
}
