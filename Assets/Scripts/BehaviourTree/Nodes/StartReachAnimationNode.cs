using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartReachAnimationNode : Node
{
    private Transform player;
    private Transform enemy;
    private Animator animator;
    private float captureDistance;
    
    public StartReachAnimationNode(Transform player, Transform enemy, Animator animator, float captureDistance)
    {
        this.player = player;
        this.enemy = enemy;
        this.animator = animator;
        this.captureDistance = captureDistance;
    }

    public override NodeState Evaluate()
    {
        if (Vector3.Distance(enemy.position, player.position) > captureDistance)
        {
            return NodeState.FAILURE;
        }
        else
        {
            animator.SetBool("GrabActionBool", true);
            return NodeState.SUCCESS;
        }
    }
}
