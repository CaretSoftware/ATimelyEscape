using System;
using System.Collections;
using System.Collections.Generic;
using NewRatCharacterController;
using UnityEngine;

public class LetGoOfCubeChristoffer : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        CubePushState.cubeLetGo?.Invoke();
    }

    private void OnCollisionEnter(Collision collision) {  
        CubePushState.cubeLetGo?.Invoke();
    }
}
