using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    protected List<Node> nodes = new List<Node>();

    public Sequence (List<Node> list)
    {
        nodes = list;
    }
    public override NodeState Evaluate()
    {
        bool isChildRunning = false;
        foreach(var node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    isChildRunning = true;
                    break;

                case NodeState.SUCCESS:
                    _nodeState = NodeState.SUCCESS;
                    break;

                case NodeState.FAILURE:
                    _nodeState = NodeState.FAILURE;
                    return _nodeState;

                default:
                    break;
            }
        }
        _nodeState = isChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return _nodeState;
    }
}
