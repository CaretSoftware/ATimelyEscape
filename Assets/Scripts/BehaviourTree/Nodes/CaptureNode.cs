using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class CaptureNode : Node
{
    private LayerMask obstacleMask;
    private EnemyAI enemy;
    private Animator animator;

    private Transform handIKTarget;
    private Transform player;
    private Vector3 losPos;

    private float captureDistance;
    private float distanceToPlayer;
    private bool recentlyCaught;

    private RaycastHit hit;

    public CaptureNode(NavMeshAgent agent, Transform player, float captureDistance, Transform agentCenterTransform,
        Animator animator, EnemyAI enemy, LayerMask obstacleMask, IKControl ikControl)
    {
        this.player = player;
        this.enemy = enemy;
        this.obstacleMask = obstacleMask;
        losPos = agentCenterTransform.localPosition + Vector3.up * 0.8f;
        this.animator = animator;
    }

    public override NodeState Evaluate()
    {
        Physics.Raycast(losPos, (player.position - losPos).normalized,
            out hit, Mathf.Infinity, obstacleMask, QueryTriggerInteraction.Ignore);
        
            if (hit.collider.gameObject.layer == player.gameObject.layer && !enemy.IsCapturing)
                return NodeState.SUCCESS;
            else
                return NodeState.FAILURE;
    }
}