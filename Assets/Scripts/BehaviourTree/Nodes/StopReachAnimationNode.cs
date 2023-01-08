using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopReachAnimationNode : Node
{
    private Transform player;
    private Transform enemy;
    private Animator animator;
    private float captureDistance;
    
    public StopReachAnimationNode(Transform player, Transform enemy, Animator animator, float captureDistance)
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
            animator.SetBool("GrabActionBool", false);
            return NodeState.FAILURE;
        }
        return NodeState.SUCCESS;
    }
}
