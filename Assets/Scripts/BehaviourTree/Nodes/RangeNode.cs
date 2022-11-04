using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeNode : Node
{
    private float range, distance;
    private Transform origin, target;
    private EnemyFOV fov;

    public RangeNode (float range, Transform origin, Transform target, EnemyFOV fov)
    {
        this.range = range;
        this.origin = origin;
        this.target = target;
        this.fov = fov;
    }

    public override NodeState Evaluate()
    {
        distance = Vector3.Distance(origin.position, target.position);
        //Debug.Log($"distance <= range: {distance <= range}, player detected: {fov.playerDetected}");
        _nodeState = distance <= range || fov.playerDetected ? NodeState.SUCCESS : NodeState.FAILURE;
        return _nodeState;
    }
}
