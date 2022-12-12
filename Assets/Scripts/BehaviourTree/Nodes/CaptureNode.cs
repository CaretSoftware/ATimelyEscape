using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class CaptureNode : Node
{
    private MultiAimConstraint multiAimConstraint;
    private ChainIKConstraint chainIKConstraint;
    private NavMeshAgent agent;
    private Animator animator;

    private Transform agentTransform;
    private Transform handIKTarget;
    private Transform checkpoint;
    private Transform player;

    private float captureDistance;
    private float destinationDistance;

    public CaptureNode(NavMeshAgent agent, Transform player, float captureDistance, Transform checkpoint,
        Transform agentTransform, Transform handIKTarget, Animator animator, MultiAimConstraint multiAimConstraint, ChainIKConstraint chainIKConstraint)
    {
        this.agent = agent;
        this.player = player;
        this.captureDistance = captureDistance;
        this.checkpoint = checkpoint;
        this.agentTransform = agentTransform;
        this.handIKTarget = handIKTarget;
        this.animator = animator;
        this.multiAimConstraint = multiAimConstraint;
        this.chainIKConstraint = chainIKConstraint;
    }

    public override NodeState Evaluate()
    {
        destinationDistance = Vector3.Distance(player.position, agentTransform.transform.position);
        //animation to lerp handIKTarget towards player position
        //if handIKTarget reach player, transform player position to checkpoint
        //if (animator.GetIKPositionWeight(AvatarIKGoal.RightHand))
        handIKTarget.position = player.position;
        animator.SetTrigger("GrabTrigger");

        if (destinationDistance < captureDistance - 0.1f)
        {
            handIKTarget.position = player.position;
                player.transform.position = checkpoint.position;
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
