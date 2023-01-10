using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class CaptureNode : Node
{
    private Vector3 PlayerColliderOffset = new Vector3(0f, 0.1f, 0f);

    private Transform player;
    private EnemyAI enemy;

    private Vector3 losPos;
    private LayerMask obstacleMask;
    private RaycastHit hit;
    private Vector3 playerColliderPos;

    public CaptureNode(Transform player, EnemyAI enemy, Vector3 losPos, LayerMask obstacleMask)
    {
        this.player = player;
        this.losPos = losPos;
        this.enemy = enemy;
        this.obstacleMask = obstacleMask;
    }

    public override NodeState Evaluate()
    {
        playerColliderPos = player.position + PlayerColliderOffset;
        Physics.Raycast(losPos, (playerColliderPos - losPos).normalized,
            out hit, Mathf.Infinity, obstacleMask, QueryTriggerInteraction.Ignore);
        //Debug.Log($"hit layer: {hit.collider.gameObject.layer}");
        if (hit.collider.gameObject.layer == player.gameObject.layer && !enemy.IsCapturing)
        {
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }
}