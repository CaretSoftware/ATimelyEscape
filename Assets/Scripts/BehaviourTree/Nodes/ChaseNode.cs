using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseNode : Node
{
    private const float triggerDistance = 0.2f;

    private Transform target;
    private Transform agentTransform;
    private NavMeshAgent agent;

    public ChaseNode(Transform target, NavMeshAgent agent, Transform agentTransform)
    {
        this.target = target;
        this.agent = agent;
        this.agentTransform = agentTransform;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Chasing");
        float dist = Vector3.Distance(target.position, agentTransform.position);
        if(dist > triggerDistance)
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
