using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class CaptureNode : Node
{
    private static DummyBehaviour Instance;
    
    private GameObject GO = new GameObject();
    private GameObject dummyBehaviourParent;
    
    private NavMeshAgent agent;
    private Animator animator;
    private EnemyAI ai;
    private Transform agentTransform;
    private Transform handIKTarget;
    private Transform player;

    private float captureDistance;
    private float destinationDistance;


    public CaptureNode(NavMeshAgent agent, Transform player, float captureDistance, Transform agentTransform, Animator animator, EnemyAI ai)
    {
        this.agent = agent;
        this.player = player;
        this.captureDistance = captureDistance;
        this.agentTransform = agentTransform;
        this.animator = animator;
        this.ai = ai;
        dummyBehaviourParent = agent.gameObject;
        Instance = GO.AddComponent<DummyBehaviour>();
        GO.transform.parent = dummyBehaviourParent.transform;
    }

    private bool recentlyCaught;
    public override NodeState Evaluate()
    {
        destinationDistance = Vector3.Distance(player.position, agentTransform.transform.position);
        //Debug.Log("Destination distance:" + destinationDistance);
        
        if (destinationDistance < captureDistance && !ai.IsCapturing && !recentlyCaught)
        {
            //Debug.Log("GrabAction triggered");
            animator.SetTrigger("GrabAction");
            agent.isStopped = true;
            ai.IsCapturing = true;
            return NodeState.FAILURE;
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            return NodeState.RUNNING;
        }
    }

    private IEnumerator RecentlyCaughtTimer()
    {
        recentlyCaught = true;
        yield return new WaitForSeconds(3f);
        recentlyCaught = false;
    }
    private class DummyBehaviour : MonoBehaviour { }
}
