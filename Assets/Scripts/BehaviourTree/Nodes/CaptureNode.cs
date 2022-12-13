using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class CaptureNode : Node
{
    private static DummyBehaviour Instance;

    private ChainIKConstraint chainIKConstraint;
    private NavMeshAgent agent;
    private Animator animator;

    private Transform agentTransform;
    private Transform handIKTarget;
    private Transform checkpoint;
    private Transform player;

    private float captureDistance;
    private float destinationDistance;

    private GameObject GO = new GameObject();
    private GameObject dummyBehaviourParent;

    public CaptureNode(NavMeshAgent agent, Transform player, float captureDistance, Transform checkpoint,
        Transform agentTransform, Transform handIKTarget, Animator animator, ChainIKConstraint chainIKConstraint)
    {
        dummyBehaviourParent = agent.gameObject;
        this.agent = agent;
        this.player = player;
        this.captureDistance = captureDistance;
        this.checkpoint = checkpoint;
        this.agentTransform = agentTransform;
        this.handIKTarget = handIKTarget;
        this.animator = animator;
        this.chainIKConstraint = chainIKConstraint;
        Instance = GO.AddComponent<DummyBehaviour>();
        GO.transform.parent = dummyBehaviourParent.transform;
    }

    public override NodeState Evaluate()
    {
        destinationDistance = Vector3.Distance(player.position, agentTransform.transform.position);

        if (destinationDistance < captureDistance - 0.1f)
        {
            handIKTarget.position = player.position;
            animator.SetTrigger("GrabAction");
            agent.isStopped = true;
            return NodeState.FAILURE;
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            return NodeState.RUNNING;
        }
    }

    private class DummyBehaviour : MonoBehaviour { }

    /*
    private IEnumerator WaitForAnimation()
    {
        handIKTarget.position = player.position;
        animator.SetTrigger("GrabAction");
        do 
            yield return null; 
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0));
    }
    */
}
