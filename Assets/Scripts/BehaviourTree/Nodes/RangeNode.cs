using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeNode : Node
{
    private float range, distanceToPlayer;
    private Transform enemy, player;
    private EnemyFOV fov;
    private Animator animator;

    public RangeNode (float range, Transform enemy, Transform player, EnemyFOV fov, Animator animator)
    {
        this.range = range;
        this.enemy = enemy;
        this.player = player;
        this.animator = animator;
        this.fov = fov;
    }

    public override NodeState Evaluate()
    {
        distanceToPlayer = Vector3.Distance(enemy.position, player.position);
        if (distanceToPlayer <= range || fov.playerDetected)
            return NodeState.SUCCESS;
        else
        {
            Debug.Log($"Stop");
            animator.SetBool("GrabActionBool", false);
            return NodeState.FAILURE;
        }
        
    }
}
