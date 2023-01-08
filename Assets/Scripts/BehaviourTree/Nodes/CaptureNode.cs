using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class CaptureNode : Node
{
    private LayerMask obstacleMask;
    private NavMeshAgent agent;
    private Animator animator;
    private EnemyAI ai;
    private IKControl ikControl;
    
    private Transform agentCenterTransform;
    private Transform handIKTarget;
    private Transform player;
    private Vector3 losPos;

    private float captureDistance;
    private float destinationDistance;
    private bool recentlyCaught;

    public CaptureNode(NavMeshAgent agent, Transform player, float captureDistance, Transform agentCenterTransform,
        Animator animator, EnemyAI ai, LayerMask obstacleMask, IKControl ikControl)
    {
        this.agent = agent;
        this.player = player;
        this.captureDistance = captureDistance;
        this.agentCenterTransform = agentCenterTransform;
        this.animator = animator;
        this.ai = ai;
        this.obstacleMask = obstacleMask;
        this.ikControl = ikControl;
        losPos = agentCenterTransform.localPosition + Vector3.up * 0.8f;
    }

    //TODO raycasta mot spelaren, om default träffad gå vidare, annars fånga spelaren.
    private RaycastHit hit;
    public override NodeState Evaluate()
    {
        destinationDistance = Vector3.Distance(player.position, agentCenterTransform.transform.position);   
        
        Physics.Raycast(losPos, (player.position - losPos).normalized,
            out hit, Mathf.Infinity, obstacleMask, QueryTriggerInteraction.Ignore);
        //Debug.Log($"hit: {hit.transform.gameObject.name},  layer: {hit.transform.gameObject.layer}, player layer: {player.gameObject.layer}");
        if (destinationDistance < captureDistance)
        {
            Debug.Log($"Within captureDistance");
            agent.isStopped = true;
            if (hit.collider.gameObject.layer == player.gameObject.layer && !ai.IsCapturing && !recentlyCaught)
            {
                animator.SetTrigger("GrabAction");
                Debug.Log($"GrabAction triggered");
                //agent.isStopped = true;
                ai.IsCapturing = true;
                return NodeState.FAILURE;
            }
            return NodeState.RUNNING;
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            return NodeState.RUNNING;
        }
    }
}