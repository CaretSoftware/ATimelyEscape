using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseNode : Node
{

    private NavMeshAgent agent;
    private Transform target;
    private Transform agentTransform;

    private float destinationDistance;
    private float triggerDistance;

    public ChaseNode(Transform target, NavMeshAgent agent, Transform agentTransform, float triggerDistance)
    {
        this.target = target;
        this.agent = agent;
        this.agentTransform = agentTransform;
        this.triggerDistance = triggerDistance;
    }

    public override NodeState Evaluate()
    {
        destinationDistance = Vector3.Distance(target.position, agentTransform.position);
        if(destinationDistance > triggerDistance)
        {
            //Debug.Log("Chasing");
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
