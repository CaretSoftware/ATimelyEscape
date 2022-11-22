using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CaptureNode : Node
{
    private NavMeshAgent agent;
    private Animator animator;

    private Transform agentTransform;
    private Transform handIKTarget;
    private Transform checkpoint;
    private Transform target;

    private float captureDistance;
    private float destinationDistance;

    public CaptureNode(NavMeshAgent agent, Transform target, float captureDistance, Transform checkpoint, 
        Transform agentTransform, Transform handIKTarget, Animator animator)
    {
        this.agent = agent;
        this.target = target;
        this.captureDistance = captureDistance;
        this.checkpoint = checkpoint;
        this.agentTransform = agentTransform;
        this.handIKTarget = handIKTarget;
        this.animator = animator;
    }

    public override NodeState Evaluate()
    {
        destinationDistance = Vector3.Distance(target.position, agentTransform.transform.position);
        if (destinationDistance < captureDistance - 0.1f)
        {
                handIKTarget.position = target.position;
                if(animator.GetIKPositionWeight(AvatarIKGoal.RightHand) > captureDistance)
                animator.SetTrigger("GrabItem");
                    //target.transform.position = checkpoint.position;
            agent.isStopped = true;
            return NodeState.FAILURE;
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            return NodeState.RUNNING;
        }
    }
}
