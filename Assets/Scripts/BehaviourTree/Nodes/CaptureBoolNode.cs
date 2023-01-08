using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureBoolNode : Node
{
    private Transform player;
    private Transform enemy;
    private Animator animator;
    private EnemyAI ai;
    private float captureRange;
    
    private static readonly int GrabActionBool = Animator.StringToHash("GrabActionBool");
    
    public CaptureBoolNode(Transform player, Transform enemy, EnemyAI ai,Animator animator, float captureRange)
    {
        this.player = player;
        this.enemy = enemy;
        this.animator = animator;
        this.captureRange = captureRange;
        this.ai = ai;
    }

    private bool withinRange;
    private float currentDistance;

    public override NodeState Evaluate()
    {
        currentDistance = Vector3.Distance(player.position, enemy.position);
        withinRange = currentDistance > captureRange;
        animator.SetBool(GrabActionBool, withinRange);

        if (withinRange)
            return NodeState.SUCCESS;
        else
            return NodeState.FAILURE;
        
    }
}
