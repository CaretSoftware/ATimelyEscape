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

    //TODO forskare avbryter inte att fånga en när man går ifrån.
    public override NodeState Evaluate()
    {
        if (Vector3.Distance(enemy.position, player.position) > captureDistance)
        {
            Debug.Log($"Stop");
            animator.SetBool("GrabActionBool", false);
            return NodeState.FAILURE;
        }
        Debug.Log($"running");
        return NodeState.SUCCESS;
    }
}
