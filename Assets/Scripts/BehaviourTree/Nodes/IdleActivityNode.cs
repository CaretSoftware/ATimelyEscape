using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleActivityNode : Node
{
    private MonoBehaviour monoBehaviour;
    private Transform[] waypoints;
    private NavMeshAgent agent;
    private EnemyAI ai;
    private System.Random random;
    private static int PREVIOUS_ACTIVITY_INDEX;

    public IdleActivityNode(Transform[] waypoints, NavMeshAgent agent, EnemyAI ai)
    {
        this.waypoints = waypoints;
        this.agent = agent;
        this.ai = ai;
    }

    public override NodeState Evaluate()
    {
        //select an index at random.
        int targetIndex = random.Next(0, waypoints.Length);
        //if the targetIndex is the last index of the array, check if it was the previous activity, if so reduce the index.
        if (targetIndex == waypoints.Length - 1)
            if(targetIndex == PREVIOUS_ACTIVITY_INDEX)
                PREVIOUS_ACTIVITY_INDEX = --targetIndex;
        //else if the target is the same as the previous activity, increase the index.
        else if (targetIndex == PREVIOUS_ACTIVITY_INDEX)
            PREVIOUS_ACTIVITY_INDEX = ++targetIndex;

        float dist = Vector3.Distance(waypoints[targetIndex].position, agent.transform.position);
        if (dist > float.Epsilon)
        {
            agent.isStopped = false;
            agent.SetDestination(waypoints[targetIndex].position);
            return NodeState.RUNNING;
        }
        else
        {
            //Perform activity here.
            //monoBehaviour.StartCoroutine(PerformActivity());
            agent.isStopped = true;
            return NodeState.SUCCESS;
        }
    }

    private IEnumerator PerformActivity()
    {
        yield return new WaitForSeconds(3f);
    }
}