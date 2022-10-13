using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseNode : Node
{
    private const float triggerDistance = 0.2f;

    private Transform target;
    private NavMeshAgent agent;
    private EnemyAI ai;

    public ChaseNode(Transform target, NavMeshAgent agent, EnemyAI ai)
    {
        this.target = target;
        this.agent = agent;
        this.ai = ai;
    }

    public override NodeState Evaluate()
    {
        ai.Color = Color.yellow;
        float dist = Vector3.Distance(target.position, agent.transform.position);
        if(dist > triggerDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            return NodeState.RUNNING;
        }
        else
        {
            agent.isStopped = true;
            return NodeState.SUCCESS;
        }
    }
}
