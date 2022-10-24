using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CaptureNode : Node
{
    private GameOverScreen gameOverScreen;
    private NavMeshAgent agent;
    private Transform target;
    private Transform agentTransform;
    private float captureDistance;
    private bool endScreenTriggered;

    private float axisOffsetX;
    private float axisOffsetZ;

    public CaptureNode(NavMeshAgent agent, Transform target, float captureDistance, GameOverScreen gameOverScreen, Transform agentTransform)
    {
        this.agent = agent;
        this.target = target;
        this.captureDistance = captureDistance;
        this.gameOverScreen = gameOverScreen;
        this.agentTransform = agentTransform;
    }


    public override NodeState Evaluate()
    {
        Debug.Log("Trying to capture");

        //axisOffsetX = Mathf.Abs(target.position.x - agentTransform.position.x);
        //axisOffsetZ = Mathf.Abs(target.position.z - agentTransform.position.z);

        float dist = Vector3.Distance(target.position, agentTransform.transform.position);
        //if (axisOffsetX < captureDistance || axisOffsetZ < captureDistance)
        if(dist < captureDistance)
        {
            Debug.Log("CAPTURED");
            if (!endScreenTriggered)
            {
                //TODO add universal variable-setup script
                gameOverScreen.FadeCanvasGroup(2);
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
