using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class CaptureNode : Node
{
    private LayerMask obstacleMask;
    private NavMeshAgent agent;
    private Animator animator;
    private EnemyAI ai;
    private Transform agentCenterTransform;
    private Transform handIKTarget;
    private Transform player;

    private float captureDistance;
    private float destinationDistance;
    private bool recentlyCaught;


    public CaptureNode(NavMeshAgent agent, Transform player, float captureDistance, Transform agentCenterTransform,
        Animator animator, EnemyAI ai, LayerMask obstacleMask)
    {
        this.agent = agent;
        this.player = player;
        this.captureDistance = captureDistance;
        this.agentCenterTransform = agentCenterTransform;
        this.animator = animator;
        this.ai = ai;
        this.obstacleMask = obstacleMask;
    }

    //TODO raycasta mot spelaren, om default träffad gå vidare, annars fånga spelaren.
    private RaycastHit hit;
    public override NodeState Evaluate()
    {
        destinationDistance = Vector3.Distance(player.position, agentCenterTransform.transform.position);
        //hit = Physics.Raycast();
            return NodeState.RUNNING;
        if (destinationDistance < captureDistance && !ai.IsCapturing && !recentlyCaught)
        {
            
            animator.SetTrigger("GrabAction");
            agent.isStopped = true;
            ai.IsCapturing = true;
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