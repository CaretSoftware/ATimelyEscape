using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class CaptureNode : Node
{
    private NavMeshAgent agent;
    private Animator animator;

    private Transform agentTransform;
    private Transform handIKTarget;
    private Transform player;

    private float captureDistance;
    private float destinationDistance;


    public CaptureNode(NavMeshAgent agent, Transform player, float captureDistance, Transform agentTransform, Transform handIKTarget, Animator animator)
    {
        this.agent = agent;
        this.player = player;
        this.captureDistance = captureDistance;
        this.agentTransform = agentTransform;
        this.handIKTarget = handIKTarget;
        this.animator = animator;
    }

    public override NodeState Evaluate()
    {
        destinationDistance = Vector3.Distance(player.position, agentTransform.transform.position);

        if (destinationDistance < captureDistance - 0.1f)
        {
            handIKTarget.position = new Vector3(player.position.x, player.position.y + 0.1f, player.position.z);
            animator.SetTrigger("GrabAction");
            agent.isStopped = true;
            return NodeState.FAILURE;
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            return NodeState.RUNNING;
        }
    }
}
