using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShootNode : Node
{
    private NavMeshAgent agent;
    private EnemyAI ai;

    public ShootNode(NavMeshAgent agent, EnemyAI ai)
    {
        this.agent = agent;
        this.ai = ai;
    }
    public override NodeState Evaluate()
    {
        agent.isStopped = false;
        ai.Color = Color.green;
        return NodeState.RUNNING;   
    }
}
