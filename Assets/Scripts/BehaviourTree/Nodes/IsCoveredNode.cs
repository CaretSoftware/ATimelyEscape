using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsCoveredNode : Node
{
    private Transform origin;
    private Transform target;

    public IsCoveredNode(Transform origin, Transform target)
    {
        this.origin = origin;
        this.target = target;
    }

    public override NodeState Evaluate()
    {
        RaycastHit hit;
        if (Physics.Raycast(origin.position, target.position - origin.position, out hit))
            if (hit.collider.transform != target)
                return NodeState.SUCCESS;
        return NodeState.FAILURE;
            
    }
}
