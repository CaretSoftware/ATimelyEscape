using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertedRangeNode : Node
{
    private float catchRange, distanceToPlayer;
    private Transform enemy, player;
    private Animator animator;
    private EnemyAI ai;

    public InvertedRangeNode (float catchRange, Transform enemy, Transform player, Animator animator, EnemyAI ai)
    {
        this.catchRange = catchRange;
        this.enemy = enemy;
        this.player = player;
        this.animator = animator;
        this.ai = ai;
    }

    public override NodeState Evaluate()
    {
        distanceToPlayer = Vector3.Distance(enemy.position, player.position);
        if (distanceToPlayer > catchRange)
        {
            animator.SetBool("GrabActionBool", false);
            Debug.Log("Stop animation");
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }
}
