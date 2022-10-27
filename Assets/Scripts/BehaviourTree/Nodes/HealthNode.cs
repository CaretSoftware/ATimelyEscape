using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthNode : Node
{
    private EnemyAI ai;
    private float threshold;

    public HealthNode(EnemyAI ai, float threshold)
    {
        this.ai = ai;
        this.threshold = threshold;
    }
    public override NodeState Evaluate()
    {
        _nodeState = ai.CurrentHealth <= threshold ? NodeState.SUCCESS : NodeState.FAILURE;
        return _nodeState;
    }
}
