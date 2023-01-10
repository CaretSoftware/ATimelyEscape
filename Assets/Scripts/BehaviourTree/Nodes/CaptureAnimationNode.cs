using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureAnimationNode : Node
{
    private Transform player;
    private Transform enemy;
    private Animator animator;
    private float captureDistance;
    
    public CaptureAnimationNode(Transform player, Transform enemy, Animator animator, float captureDistance)
    {
        this.player = player;
        this.enemy = enemy;
        this.animator = animator;
        this.captureDistance = captureDistance;
    }

    public override NodeState Evaluate()
    {
        if (Vector3.Distance(enemy.position, player.position) < captureDistance)
        {
            animator.SetBool("GrabActionBool", true);
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }
}
