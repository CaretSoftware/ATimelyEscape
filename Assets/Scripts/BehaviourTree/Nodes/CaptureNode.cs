using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CaptureNode : Node
{
    //private GameOverScreen gameOverScreen;
    private Transform checkpoint;
    private NavMeshAgent agent;
    private Transform target;
    private Transform agentTransform;

    private float captureDistance;
    private float destinationDistance;
    private bool endScreenTriggered;

    public CaptureNode(NavMeshAgent agent, Transform target, float captureDistance, Transform checkpoint, Transform agentTransform)
    {
        this.agent = agent;
        this.target = target;
        this.captureDistance = captureDistance;
        this.checkpoint = checkpoint;
        //this.gameOverScreen = gameOverScreen;
        this.agentTransform = agentTransform;
    }


    public override NodeState Evaluate()
    {
        Debug.Log("Trying to capture");
        destinationDistance = Vector3.Distance(target.position, agentTransform.transform.position);
        if (destinationDistance < captureDistance)
        {
            Debug.Log("CAPTURED");
            if (!endScreenTriggered)
            {
                //TODO update this when new checkpoint system is in place.
                target.transform.position = checkpoint.position;
                //gameOverScreen.FadeCanvasGroup(2);
                endScreenTriggered = true;
            }
            agent.isStopped = true;
            return NodeState.SUCCESS;
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            return NodeState.RUNNING;
        }
    }
}
