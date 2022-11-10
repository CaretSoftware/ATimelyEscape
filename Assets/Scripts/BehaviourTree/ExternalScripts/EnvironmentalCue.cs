using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class EnvironmentalCue : MonoBehaviour
{
    private NavMeshAgent agent;
    private LineRenderer lr;
    private NavMeshPath path;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (agent == null || agent.path == null)
            return;

        path = agent.path;
        lr.positionCount = path.corners.Length;
        for (int i = 0; i < path.corners.Length; i++)
            lr.SetPosition(i, path.corners[i]);
    }
}
