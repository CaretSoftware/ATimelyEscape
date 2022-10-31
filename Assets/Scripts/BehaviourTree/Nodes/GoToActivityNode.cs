using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoToActivityNode : Node
{
    private static DummyBehaviour Instance;
    private const float DestinationOffset = 0.3f;

    private System.Random random = new System.Random();
    private GameObject GO = new GameObject();
    private Transform[] waypoints;
    private NavMeshAgent agent;
    private Animator animator;

    private float idleTimer;
    private float destinationDistance;
    private int targetIndex = 0;

    private bool isCoroutineRunning;
    private bool isTimerDone;
    
    public GoToActivityNode(Transform[] waypoints, NavMeshAgent agent, Animator animator, float idleTimer)
    {
        this.waypoints = waypoints;
        this.agent = agent;
        this.animator = animator;
        this.idleTimer = idleTimer;
        Instance = GO.AddComponent<DummyBehaviour>();
    }

    public override NodeState Evaluate()
    {
        destinationDistance = Vector3.Distance(waypoints[targetIndex].position, agent.transform.position);

        if (destinationDistance > DestinationOffset)
        {
            isTimerDone = false;
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
            GenerateRandomTargetIndex(waypoints.Length - 1);
            agent.isStopped = true;
            return NodeState.SUCCESS;
        }
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

    private void GenerateRandomTargetIndex(int max)
    {
        int value = random.Next(0, max);
        if (value == targetIndex && value != max)
            value++;
        else if (value == targetIndex && value == max)
            value--;
        targetIndex = value;
    }

    private class DummyBehaviour : MonoBehaviour { }
}