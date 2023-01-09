using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class ChaseNode : Node
{
    private NavMeshAgent agent;
    private Transform player;
    private Transform agentTransform;

    private float distanceToPlayer;
    private float captureDistance;
    private float chaseDistance;

    public ChaseNode(Transform player, NavMeshAgent agent, Transform agentTransform, float captureDistance, float chaseDistance)
    {
        this.player = player;
        this.agent = agent;
        this.agentTransform = agentTransform;
        this.captureDistance = captureDistance;
        this.chaseDistance = chaseDistance;
    }

    public override NodeState Evaluate()
    {
        distanceToPlayer = Vector3.Distance(player.position, agentTransform.position);
        if(distanceToPlayer > captureDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            return NodeState.RUNNING;
        }
        else
            return NodeState.FAILURE;
    }
}
