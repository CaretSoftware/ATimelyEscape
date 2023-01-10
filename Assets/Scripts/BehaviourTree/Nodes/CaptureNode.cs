using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class CaptureNode : Node
{
    private Transform player;
    private EnemyAI enemy;

    private Vector3 losPos;
    private LayerMask obstacleMask;
    private RaycastHit hit;

    public CaptureNode(Transform player, EnemyAI enemy, Vector3 losPos, LayerMask obstacleMask)
    {
        this.player = player;
        this.losPos = losPos;
        this.enemy = enemy;
        this.obstacleMask = obstacleMask;
    }

    public override NodeState Evaluate()
    {
        Physics.Raycast(losPos, (player.position - losPos).normalized,
            out hit, Mathf.Infinity, obstacleMask, QueryTriggerInteraction.Ignore);

        if (hit.collider.gameObject.layer == player.gameObject.layer && !enemy.IsCapturing)
        {
            enemy.DrawLOS(true);
            return NodeState.SUCCESS;
        }
        else
        {
            
            enemy.DrawLOS(false);
            return NodeState.FAILURE;
        }
    }
}