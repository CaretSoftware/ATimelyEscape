using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureBoolNode : Node
{
    private Transform player;
    private Transform enemy;
    private Animator animator;
    private EnemyAI ai;
    private float range;
    
    private static readonly int GrabActionBool = Animator.StringToHash("GrabActionBool");
    
    public CaptureBoolNode(Transform player, Transform enemy, EnemyAI ai,Animator animator, float range)
    {
        this.player = player;
        this.enemy = enemy;
        this.animator = animator;
        this.range = range;
        this.ai = ai;
    }

    public override NodeState Evaluate()
    {
        if (Vector3.Distance(player.position, enemy.position) > range)
        {
            animator.SetBool(GrabActionBool, true);
            return NodeState.SUCCESS;
        }
        else
        {
            animator.SetBool(GrabActionBool, false);
            return NodeState.FAILURE;
        }
    }
}
