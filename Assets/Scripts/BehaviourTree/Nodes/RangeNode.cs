using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeNode : Node
{
    private float range;
    private Transform origin, target;

    public RangeNode (float range, Transform origin, Transform target)
    {
        this.range = range;
        this.origin = origin;
        this.target = target;
    }

    public override NodeState Evaluate()
    {
        float dist = Vector3.Distance(origin.position, target.position);
        _nodeState = dist <= range ? NodeState.SUCCESS : NodeState.FAILURE;
        return _nodeState;

    }
}
