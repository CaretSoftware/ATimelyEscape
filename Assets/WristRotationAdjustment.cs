using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristRotationAdjustment : MonoBehaviour
{
    private Transform Wrist;
    private void Start()
    {
        Wrist = GameObject.Find($"{transform.root.name}" +
            $"/mixamorig:Hips" +
            $"/mixamorig:Spine" +
            $"/mixamorig:Spine1" +
            $"/mixamorig:Spine2" +
            $"/mixamorig:LeftShoulder" +
            $"/mixamorig:LeftArm" +
            $"/mixamorig:LeftForeArm").transform;
    }

    void Update()
    {
        transform.rotation = Wrist.rotation;
    }
}
