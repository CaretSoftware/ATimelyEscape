using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class CaptureNode : Node
{
    private NavMeshAgent agent;
    private Animator animator;
    private EnemyAI ai;
    private Transform agentTransform;
    private Transform handIKTarget;
    private Transform player;

    private float captureDistance;
    private float destinationDistance;


    public CaptureNode(NavMeshAgent agent, Transform player, float captureDistance, Transform agentTransform, Animator animator, EnemyAI ai)
    {
        this.agent = agent;
        this.player = player;
        this.captureDistance = captureDistance;
        this.agentTransform = agentTransform;
        this.animator = animator;
        this.ai = ai;
    }

    public override NodeState Evaluate()
    {
        destinationDistance = Vector3.Distance(player.position, agentTransform.transform.position);

        if (destinationDistance < captureDistance && !ai.IsReaching)
        {
            animator.SetTrigger("GrabAction");
            agent.isStopped = true;
            ai.IsReaching = true;
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
