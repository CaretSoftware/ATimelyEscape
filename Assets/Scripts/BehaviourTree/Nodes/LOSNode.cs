using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOSNode : Node
{
    private Transform agent;
    private Transform player;
    private LayerMask layerMask;
    private GameObject playerTag;

    public LOSNode(Transform player, Transform agent, LayerMask layerMask)
    {
        this.agent = agent;
        this.player = player;
        this.layerMask = layerMask;
        playerTag = GameObject.FindGameObjectWithTag("Player");
    }

    private float distance;
    private RaycastHit hit;

    public override NodeState Evaluate()
    {
        distance = Vector3.Distance(player.position, agent.transform.position);
        Physics.Raycast(agent.position, (player.position - agent.position).normalized,
            out hit, distance, layerMask, QueryTriggerInteraction.Ignore);
        Debug.Log($"hit: {hit.transform.gameObject.name},  layer: {hit.transform.gameObject.layer}, player layer: {player.gameObject.layer}");
        if (hit.collider.gameObject.layer == player.gameObject.layer)
        {
            Debug.Log($"success");
            return NodeState.SUCCESS;
        }
        else
            return NodeState.RUNNING;
    }
}