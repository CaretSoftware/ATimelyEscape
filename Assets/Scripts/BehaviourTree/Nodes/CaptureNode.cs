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
    private float distanceToPlayer;
    private bool recentlyCaught;

    private RaycastHit hit;
    private static readonly int GrabActionBool = Animator.StringToHash("GrabActionBool");

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

    public override NodeState Evaluate()
    {
        //distanceToPlayer = Vector3.Distance(player.position, agentCenterTransform.transform.position);

        Physics.Raycast(losPos, (player.position - losPos).normalized,
            out hit, Mathf.Infinity, obstacleMask, QueryTriggerInteraction.Ignore);
        
            if (hit.collider.gameObject.layer == player.gameObject.layer && !ai.IsCapturing && !recentlyCaught)
            {
                //animator.SetBool(GrabActionBool, true);
                //animator.SetTrigger("GrabAction");
                //ai.IsCapturing = true;
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
    }
}