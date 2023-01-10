using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraTarget : MonoBehaviour
{
    public LookAt lookAtScript;
    [SerializeField] private Transform newCameraTarget;

    private void OnTriggerEnter(Collider player) {
        if (player.tag == "Player"){
            lookAtScript = player.GetComponentInParent<LookAt>();
            lookAtScript.cameraTarget = newCameraTarget;
        }
    }
}
