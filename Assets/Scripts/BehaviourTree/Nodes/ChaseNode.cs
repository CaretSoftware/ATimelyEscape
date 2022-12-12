using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class ChaseNode : Node
{

    private MultiAimConstraint multiAimConstraint;
    private NavMeshAgent agent;
    private Transform target;
    private Transform agentTransform;

    private float destinationDistance;
    private float triggerDistance;

    public ChaseNode(Transform target, NavMeshAgent agent, Transform agentTransform, float triggerDistance, MultiAimConstraint multiAimConstraint)
    {
        this.target = target;
        this.agent = agent;
        this.agentTransform = agentTransform;
        this.triggerDistance = triggerDistance;
        this.multiAimConstraint = multiAimConstraint;
    }

    public override NodeState Evaluate()
    {
        destinationDistance = Vector3.Distance(target.position, agentTransform.position);
        if(destinationDistance > triggerDistance)
        {
            //Debug.Log("Chasing");
            agent.isStopped = false;
            multiAimConstraint.weight = 1;
            agent.SetDestination(target.position);
            return NodeState.RUNNING;
        }
        else
        {
            multiAimConstraint.weight = 0;
            agent.isStopped = true;
            return NodeState.SUCCESS;
        }
    }
}
