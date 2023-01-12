using System;
using System.Collections;
using System.Collections.Generic;
using NewRatCharacterController;
using UnityEngine;

public class LetGoOfCubeChristoffer : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        Debug.Log($"PlayerCollision {other.transform.name} {CubePushState.cubeLetGo != null}");
        CubePushState.cubeLetGo?.Invoke();
    }

    private void OnCollisionEnter(Collision collision) {  
        Debug.Log($"PlayerCollision {collision.transform.name} {CubePushState.cubeLetGo != null}");
        CubePushState.cubeLetGo?.Invoke();
    }
}
