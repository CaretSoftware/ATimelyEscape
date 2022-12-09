using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour

{

    public Transform target;
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
        transform.LookAt(target);

            if(Input.GetKeyDown(KeyCode.RightShift)){
            thirdPersonCamera.enabled = false;
            exitCamera.enabled = true;
        }
        if(Input.GetKeyUp(KeyCode.RightShift)){
            exitCamera.enabled = false;
            thirdPersonCamera.enabled = true;
            }
    }
}
