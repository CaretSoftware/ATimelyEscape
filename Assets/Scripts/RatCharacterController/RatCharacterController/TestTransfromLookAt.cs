using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTransfromLookAt : MonoBehaviour {
    [ContextMenu(nameof(Test))]
    public void Test() {
        NewRatCameraController.SetLookTarget(this.transform, new Vector3(-.4f, .1f, -.2f), 3f, 2f);
    }
}

