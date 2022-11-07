using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoToActivityNode : Node
{
    private static DummyBehaviour Instance;
    private const float DestinationOffset = 0.5f;

    private System.Random random = new System.Random();
    private GameObject GO = new GameObject();
    private GameObject dummyBehaviourParent;
    private Transform[] waypoints;
    private NavMeshAgent agent;
    private Animator animator;

    private float idleTimer;
    private float destinationDistance;
    private int targetIndex = 0;
    private int prevIndex;

    private bool isCoroutineRunning;
    private bool isTimerDone;

    
    public GoToActivityNode(Transform[] waypoints, NavMeshAgent agent, Animator animator, GameObject parentObject, float idleTimer)
    {
        dummyBehaviourParent = parentObject;
        this.waypoints = waypoints;
        this.agent = agent;
        this.animator = animator;
        this.idleTimer = idleTimer;
        Instance = GO.AddComponent<DummyBehaviour>();
        GO.transform.parent = dummyBehaviourParent.transform;
    }

    public override NodeState Evaluate()
    {
        destinationDistance = Vector3.Distance(waypoints[targetIndex].position, agent.transform.position);

        if (destinationDistance > DestinationOffset)
        {
            isTimerDone = false;
            facingTarget = false;
            agent.isStopped = false;
            agent.SetDestination(waypoints[targetIndex].position);
            return NodeState.RUNNING;
        }
        else if (destinationDistance < DestinationOffset && !isTimerDone)
        {
                if (!isCoroutineRunning)
                    Instance.StartCoroutine(Timer());              
            return NodeState.RUNNING;
        }
        else
        {
            GenerateRandomTargetIndex(waypoints.Length);
            prevIndex = targetIndex;
            agent.isStopped = true;
            return NodeState.SUCCESS;
        }
    }

    private void GenerateRandomTargetIndex(int max)
    {
        int value = random.Next(0, max);
        if (value == prevIndex)
            GenerateRandomTargetIndex(max);
        else
            targetIndex = value;
    }

    private IEnumerator Timer()
    {
        isCoroutineRunning = true;
        animator.SetBool("move", false);
        yield return new WaitForSeconds(idleTimer);
        animator.SetBool("move", true);
        isCoroutineRunning = false;
        isTimerDone = true;
    }

    private Vector3 targetRotation;
    private float faceActivityTimer;
    private float elapsedTime;
    private bool facingTarget;

    /*
    private IEnumerator FaceTarget()
    {
        elapsedTime = 0;
        targetRotation = waypoints[targetIndex].forward;
        Quaternion rotation = Quaternion.LookRotation(targetRotation);

        while (elapsedTime < faceActivityTimer)
        {
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, rotation, (elapsedTime / faceActivityTimer));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return 0;
        facingTarget = true;
    }
    */

    private class DummyBehaviour : MonoBehaviour { }
}