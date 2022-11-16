using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class EnvironmentalCue : MonoBehaviour
{
    //reassign after entering new room.
    [SerializeField] private ObjectiveHolder objectiveHolder;
    private LineRenderer lr;
    private NavMeshPath path;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        path = new NavMeshPath();
    }

    private void Update()
    {
        if ((Input.GetKey("space") && objectiveHolder.currentObjective != Vector3.zero))
        {
            lr.enabled = true;
            NavMesh.CalculatePath(transform.position, objectiveHolder.currentObjective, NavMesh.AllAreas, path);
            lr.positionCount = path.corners.Length;
            for (int i = 0; i < path.corners.Length; i++)
                lr.SetPosition(i, path.corners[i]);
        }
        else
            lr.enabled = false;
    }
}
