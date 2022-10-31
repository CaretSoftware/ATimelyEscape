using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseNode : Node
{
    private const float triggerDistance = 0.2f;

    private NavMeshAgent agent;
    private Transform target;
    private Transform agentTransform;

    private float destinationDistance;

    public ChaseNode(Transform target, NavMeshAgent agent, Transform agentTransform)
    {
        this.target = target;
        this.agent = agent;
        this.agentTransform = agentTransform;
    }

    public override NodeState Evaluate()
    {
        destinationDistance = Vector3.Distance(target.position, agentTransform.position);
        if(destinationDistance > triggerDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            return NodeState.RUNNING;
        }
        else
        {
            agent.isStopped = true;
            return NodeState.SUCCESS;
        }
    }
}
