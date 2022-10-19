using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoToActivityNode : Node
{
    private const float DestinationOffset = 0.2f;

    private Transform[] waypoints;
    private NavMeshAgent agent;
    private int targetIndex = 0;

    public GoToActivityNode(Transform[] waypoints, NavMeshAgent agent)
    {
        this.waypoints = waypoints;
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        float dist = Vector3.Distance(waypoints[targetIndex].position, agent.transform.position);
        if (dist > DestinationOffset)
        {
            agent.isStopped = false;
            agent.SetDestination(waypoints[targetIndex].position);
            return NodeState.RUNNING;
        }
        else
        {
            targetIndex++;
            if (targetIndex == waypoints.Length - 1)
                targetIndex = 0;

            Debug.Log($"targetIndex updated to: {targetIndex}");
            agent.isStopped = true;
            return NodeState.SUCCESS;
        }
    }
}