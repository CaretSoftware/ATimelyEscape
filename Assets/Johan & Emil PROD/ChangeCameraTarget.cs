using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraTarget : MonoBehaviour
{
    // Start is called before the first frame update
    public LookAt lookAtScript;
    [SerializeField] private Transform newCameraTarget;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider player){
        if (player.tag == "Player"){
            lookAtScript.cameraTarget = newCameraTarget;
        }
    }
}
