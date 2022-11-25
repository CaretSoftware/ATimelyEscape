using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Cable : MonoBehaviour
{
    private const int SEGMENT_COUNT = 50;
    private Vector3[] joints;
    private LineRenderer line;

    private int curveCount;

    void Start()
    {
        line ??= GetComponent<LineRenderer>();
        line.positionCount = transform.childCount;
        joints = new Vector3[line.positionCount];
        curveCount = (int)joints.Length / 3;
        AssignJoints();
    }

    private void Update()
    {
        line.SetPositions(joints);
    }

    private void AssignJoints()
    {
        for (int i = 0; i < joints.Length; i++)
            joints[i] = transform.GetChild(i).GetComponent<Transform>().position;
    }
}
