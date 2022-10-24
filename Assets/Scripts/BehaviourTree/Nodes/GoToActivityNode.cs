using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoToActivityNode : Node
{
    private static DummyBehaviour Instance;
    private const float DestinationOffset = 0.05f;

    private GameObject GO = new GameObject();
    private Transform[] waypoints;
    private NavMeshAgent agent;

    private bool coroutineRunning;
    private bool timerDone;
    private int targetIndex = 0;
    private float idleTime;


    public GoToActivityNode(Transform[] waypoints, NavMeshAgent agent, Animator animator, float idleTime)
    {
        this.waypoints = waypoints;
        this.agent = agent;
        this.idleTime = idleTime;
        Instance = GO.AddComponent<DummyBehaviour>();
    }

    public override NodeState Evaluate()
    {
        float dist = Vector3.Distance(waypoints[targetIndex].position, agent.transform.position);

        if (dist > DestinationOffset)
        {
            timerDone = false;
            agent.isStopped = false;
            agent.SetDestination(waypoints[targetIndex].position);
            return NodeState.RUNNING;
        }
        else if (dist < DestinationOffset && !timerDone)
        {
            if(!coroutineRunning)
                Instance.StartCoroutine(Timer());
            return NodeState.RUNNING;
        }
        else
        {
            targetIndex++;
            if (targetIndex == waypoints.Length)
                targetIndex = 0;

            agent.isStopped = true;
            return NodeState.SUCCESS;
        }
    }

    private IEnumerator Timer()
    {
        coroutineRunning = true;
        yield return new WaitForSeconds(idleTime);
        coroutineRunning = false;
        timerDone = true;
    }

    private class DummyBehaviour : MonoBehaviour { }
}